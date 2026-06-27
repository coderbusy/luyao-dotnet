using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LuYao;

/// <summary>
/// Provides a collection of static methods for argument validation.
/// Validates arguments to reduce boilerplate parameter checking code.
/// </summary>
[DebuggerStepThrough]
public static class Check
{
    /// <summary>
    /// Checks if the provided value is not null and throws an <see cref="ArgumentNullException"/> if it is.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <param name="value">The value to check for null.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated value if not null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static T NotNull<T>(T? value, string parameterName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided value is not null and throws an <see cref="ArgumentNullException"/> with a custom message if it is.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <param name="value">The value to check for null.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="message">The custom error message.</param>
    /// <returns>The validated value if not null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static T NotNull<T>(T? value, string parameterName, string message)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName, message);
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided string is not null and validates its length constraints.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="maxLength">The maximum allowed length of the string. Defaults to <see cref="int.MaxValue"/>.</param>
    /// <param name="minLength">The minimum allowed length of the string. Defaults to 0.</param>
    /// <returns>The validated string if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is null, exceeds maxLength, or is shorter than minLength.</exception>
    public static string NotNull(
        string? value,
        string parameterName,
        int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (value == null)
        {
            throw new ArgumentException($"{parameterName} can not be null!", parameterName);
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided string is not null, empty, or whitespace and validates its length constraints.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="maxLength">The maximum allowed length of the string. Defaults to <see cref="int.MaxValue"/>.</param>
    /// <param name="minLength">The minimum allowed length of the string. Defaults to 0.</param>
    /// <returns>The validated string if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is null, empty, whitespace, exceeds maxLength, or is shorter than minLength.</exception>
    public static string NotNullOrWhiteSpace(
        string? value,
        string parameterName,
        int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
        }

        if (value!.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value!.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided string is not null or empty and validates its length constraints.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="maxLength">The maximum allowed length of the string. Defaults to <see cref="int.MaxValue"/>.</param>
    /// <param name="minLength">The minimum allowed length of the string. Defaults to 0.</param>
    /// <returns>The validated string if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is null, empty, exceeds maxLength, or is shorter than minLength.</exception>
    public static string NotNullOrEmpty(
        string? value,
        string parameterName,
        int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
        }

        if (value!.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value!.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided collection is not null or empty.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="value">The collection to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated collection if not null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown when the collection is null or empty.</exception>
    public static ICollection<T> NotNullOrEmpty<T>(
        ICollection<T>? value,
        string parameterName)
    {
        if (value == null || value.Count <= 0)
        {
            throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided type is assignable to the specified base type.
    /// </summary>
    /// <typeparam name="TBaseType">The base type to check assignability against.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated type if assignable.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the type is not assignable to <typeparamref name="TBaseType"/>.</exception>
    public static Type AssignableTo<TBaseType>(
        Type type,
        string parameterName)
    {
        NotNull(type, parameterName);

        if (!typeof(TBaseType).IsAssignableFrom(type))
        {
            throw new ArgumentException($"{parameterName} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).FullName}!");
        }

        return type;
    }

    /// <summary>
    /// Validates the length of a string with optional minimum and maximum constraints.
    /// Allows null values unless minLength is greater than 0.
    /// </summary>
    /// <param name="value">The string value to check. Can be null if minLength is 0.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="maxLength">The maximum allowed length of the string.</param>
    /// <param name="minLength">The minimum allowed length of the string. Defaults to 0.</param>
    /// <returns>The validated string if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when the string violates the length constraints.</exception>
    public static string? Length(
        string? value,
        string parameterName,
        int maxLength,
        int minLength = 0)
    {
        if (minLength > 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            if (value!.Length < minLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }
        }

        if (value != null && value.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided 16-bit signed integer is positive (greater than zero).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated value if positive.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static Int16 Positive(
        Int16 value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// Checks if the provided 32-bit signed integer is positive (greater than zero).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated value if positive.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static Int32 Positive(
        Int32 value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// Checks if the provided 64-bit signed integer is positive (greater than zero).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated value if positive.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static Int64 Positive(
        Int64 value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// Checks if the provided single-precision floating-point number is positive (greater than zero).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated value if positive.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static float Positive(
        float value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// Checks if the provided double-precision floating-point number is positive (greater than zero).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated value if positive.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static double Positive(
        double value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// Checks if the provided decimal number is positive (greater than zero).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The validated value if positive.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static decimal Positive(
        decimal value,
        string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentException($"{parameterName} is equal to zero");
        }
        else if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    /// <summary>
    /// Checks if the provided 16-bit signed integer is within the specified range.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="minimumValue">The minimum allowed value (inclusive).</param>
    /// <param name="maximumValue">The maximum allowed value (inclusive). Defaults to <see cref="Int16.MaxValue"/>.</param>
    /// <returns>The validated value if within range.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is outside the specified range.</exception>
    public static Int16 Range(
        Int16 value,
        string parameterName,
        Int16 minimumValue,
        Int16 maximumValue = Int16.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided 32-bit signed integer is within the specified range.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="minimumValue">The minimum allowed value (inclusive).</param>
    /// <param name="maximumValue">The maximum allowed value (inclusive). Defaults to <see cref="Int32.MaxValue"/>.</param>
    /// <returns>The validated value if within range.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is outside the specified range.</exception>
    public static Int32 Range(
        Int32 value,
        string parameterName,
        Int32 minimumValue,
        Int32 maximumValue = Int32.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided 64-bit signed integer is within the specified range.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="minimumValue">The minimum allowed value (inclusive).</param>
    /// <param name="maximumValue">The maximum allowed value (inclusive). Defaults to <see cref="Int64.MaxValue"/>.</param>
    /// <returns>The validated value if within range.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is outside the specified range.</exception>
    public static Int64 Range(
        Int64 value,
        string parameterName,
        Int64 minimumValue,
        Int64 maximumValue = Int64.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided single-precision floating-point number is within the specified range.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="minimumValue">The minimum allowed value (inclusive).</param>
    /// <param name="maximumValue">The maximum allowed value (inclusive). Defaults to <see cref="float.MaxValue"/>.</param>
    /// <returns>The validated value if within range.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is outside the specified range.</exception>
    public static float Range(
        float value,
        string parameterName,
        float minimumValue,
        float maximumValue = float.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }
        return value;
    }

    /// <summary>
    /// Checks if the provided double-precision floating-point number is within the specified range.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="minimumValue">The minimum allowed value (inclusive).</param>
    /// <param name="maximumValue">The maximum allowed value (inclusive). Defaults to <see cref="double.MaxValue"/>.</param>
    /// <returns>The validated value if within range.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is outside the specified range.</exception>
    public static double Range(
        double value,
        string parameterName,
        double minimumValue,
        double maximumValue = double.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided decimal number is within the specified range.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <param name="minimumValue">The minimum allowed value (inclusive).</param>
    /// <param name="maximumValue">The maximum allowed value (inclusive). Defaults to <see cref="decimal.MaxValue"/>.</param>
    /// <returns>The validated value if within range.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is outside the specified range.</exception>
    public static decimal Range(
        decimal value,
        string parameterName,
        decimal minimumValue,
        decimal maximumValue = decimal.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    /// <summary>
    /// Checks if the provided nullable struct value is not null and not equal to its default value.
    /// </summary>
    /// <typeparam name="T">The struct type of the value to check.</typeparam>
    /// <param name="value">The nullable value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The non-null struct value if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is null or equals the default value for the struct.</exception>
    public static T NotDefaultOrNull<T>(
        T? value,
        string parameterName)
        where T : struct
    {
        if (value == null)
        {
            throw new ArgumentException($"{parameterName} is null!", parameterName);
        }

        if (value.Value.Equals(default(T)))
        {
            throw new ArgumentException($"{parameterName} has a default value!", parameterName);
        }

        return value.Value;
    }
}
