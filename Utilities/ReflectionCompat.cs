using System;
using System.Reflection;

namespace Undefined.Utilities;

internal static class ReflectionCompat
{
	private const BindingFlags AllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	public static T GetField<T>(object target, string name, T fallback = default)
	{
		if (target == null)
		{
			return fallback;
		}

		FieldInfo field = FindField(target.GetType(), name);
		object value = field != null ? field.GetValue(target) : FindProperty(target.GetType(), name)?.GetValue(target);
		return value is T typed ? typed : fallback;
	}

	public static T GetStaticField<T>(Type type, string name, T fallback = default)
	{
		FieldInfo field = FindField(type, name);
		object value = field != null ? field.GetValue(null) : FindProperty(type, name)?.GetValue(null);
		return value is T typed ? typed : fallback;
	}

	public static bool SetField(object target, string name, object value)
	{
		if (target == null)
		{
			return false;
		}

		FieldInfo field = FindField(target.GetType(), name);
		if (field == null)
		{
			PropertyInfo property = FindProperty(target.GetType(), name);
			if (property == null || !property.CanWrite)
			{
				return false;
			}

			property.SetValue(target, ConvertValue(value, property.PropertyType));
			return true;
		}

		field.SetValue(target, ConvertValue(value, field.FieldType));
		return true;
	}

	public static bool SetStaticField(Type type, string name, object value)
	{
		FieldInfo field = FindField(type, name);
		if (field == null)
		{
			PropertyInfo property = FindProperty(type, name);
			if (property == null || !property.CanWrite)
			{
				return false;
			}

			property.SetValue(null, ConvertValue(value, property.PropertyType));
			return true;
		}

		field.SetValue(null, ConvertValue(value, field.FieldType));
		return true;
	}

	public static object Invoke(object target, string name, params object[] arguments)
	{
		if (target == null)
		{
			return null;
		}

		return InvokeCore(target.GetType(), target, name, arguments);
	}

	public static object InvokeStatic(Type type, string name, params object[] arguments)
	{
		if (type == null)
		{
			return null;
		}

		return InvokeCore(type, null, name, arguments);
	}

	public static Type FindType(string fullName)
	{
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			Type type = assembly.GetType(fullName, false);
			if (type != null)
			{
				return type;
			}
		}

		return null;
	}

	private static object InvokeCore(Type type, object target, string name, object[] arguments)
	{
		MethodInfo method = null;
		object[] converted = null;
		int bestScore = -1;
		for (Type current = type; current != null; current = current.BaseType)
		{
			foreach (MethodInfo candidate in current.GetMethods(AllMembers | BindingFlags.DeclaredOnly))
			{
				if (candidate.Name == name && !candidate.ContainsGenericParameters && TryConvertArguments(candidate, arguments, out object[] candidateArguments, out int score) && score > bestScore)
				{
					method = candidate;
					converted = candidateArguments;
					bestScore = score;
				}
			}
		}

		if (method == null)
		{
			return null;
		}

		return method.Invoke(target, converted);
	}

	private static bool TryConvertArguments(MethodInfo method, object[] arguments, out object[] converted, out int score)
	{
		ParameterInfo[] parameters = method.GetParameters();
		converted = null;
		score = 0;
		if (parameters.Length != arguments.Length)
		{
			return false;
		}

		object[] values = new object[arguments.Length];
		for (int index = 0; index < arguments.Length; index++)
		{
			Type parameterType = parameters[index].ParameterType;
			if (parameterType.IsByRef)
			{
				parameterType = parameterType.GetElementType();
			}

			object argument = arguments[index];
			if (argument == null)
			{
				if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
				{
					return false;
				}
				values[index] = null;
				score++;
				continue;
			}

			Type argumentType = argument.GetType();
			if (parameterType == argumentType)
			{
				values[index] = argument;
				score += 4;
				continue;
			}
			if (parameterType.IsAssignableFrom(argumentType))
			{
				values[index] = argument;
				score += 3;
				continue;
			}

			try
			{
				values[index] = ConvertValue(argument, parameterType);
				score++;
			}
			catch (Exception)
			{
				return false;
			}
		}

		converted = values;
		return true;
	}

	private static FieldInfo FindField(Type type, string name)
	{
		for (Type current = type; current != null; current = current.BaseType)
		{
			FieldInfo field = current.GetField(name, AllMembers);
			if (field != null)
			{
				return field;
			}
		}

		return null;
	}

	private static PropertyInfo FindProperty(Type type, string name)
	{
		for (Type current = type; current != null; current = current.BaseType)
		{
			PropertyInfo property = current.GetProperty(name, AllMembers);
			if (property != null)
			{
				return property;
			}
		}

		return null;
	}

	private static object ConvertValue(object value, Type targetType)
	{
		Type valueType = Nullable.GetUnderlyingType(targetType) ?? targetType;
		if (value == null || valueType.IsInstanceOfType(value))
		{
			return value;
		}

		if (valueType.IsEnum)
		{
			return Enum.ToObject(valueType, value);
		}

		return Convert.ChangeType(value, valueType);
	}
}