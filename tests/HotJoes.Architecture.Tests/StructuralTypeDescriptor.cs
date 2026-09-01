namespace HotJoes.Architecture.Tests;

public sealed class StructuralTypeDescriptor
{
    private StructuralTypeDescriptor(
        string fullName,
        string assemblyName,
        StructuralTypeKind kind,
        IEnumerable<string>? publicSettableProperties,
        IEnumerable<string>? referencedTypeAssemblies,
        IEnumerable<string>? referencedTypeNames,
        IEnumerable<string>? implementedInterfaces)
    {
        FullName = RequireValue(fullName, nameof(fullName));
        AssemblyName = RequireValue(assemblyName, nameof(assemblyName));
        Kind = kind;
        PublicSettableProperties = Normalize(publicSettableProperties);
        ReferencedTypeAssemblies = Normalize(referencedTypeAssemblies);
        ReferencedTypeNames = Normalize(referencedTypeNames);
        ImplementedInterfaces = Normalize(implementedInterfaces);
    }

    public string FullName { get; }

    public string AssemblyName { get; }

    public StructuralTypeKind Kind { get; }

    public IReadOnlyList<string> PublicSettableProperties { get; }

    public IReadOnlyList<string> ReferencedTypeAssemblies { get; }

    public IReadOnlyList<string> ReferencedTypeNames { get; }

    public IReadOnlyList<string> ImplementedInterfaces { get; }

    public static StructuralTypeDescriptor Class(
        string fullName,
        string assemblyName,
        IEnumerable<string>? publicSettableProperties = null,
        IEnumerable<string>? referencedTypeAssemblies = null,
        IEnumerable<string>? referencedTypeNames = null,
        IEnumerable<string>? implementedInterfaces = null)
    {
        return new StructuralTypeDescriptor(
            fullName,
            assemblyName,
            StructuralTypeKind.Class,
            publicSettableProperties,
            referencedTypeAssemblies,
            referencedTypeNames,
            implementedInterfaces);
    }

    public static StructuralTypeDescriptor Record(
        string fullName,
        string assemblyName,
        IEnumerable<string>? publicSettableProperties = null,
        IEnumerable<string>? referencedTypeAssemblies = null,
        IEnumerable<string>? referencedTypeNames = null,
        IEnumerable<string>? implementedInterfaces = null)
    {
        return new StructuralTypeDescriptor(
            fullName,
            assemblyName,
            StructuralTypeKind.Record,
            publicSettableProperties,
            referencedTypeAssemblies,
            referencedTypeNames,
            implementedInterfaces);
    }

    public static StructuralTypeDescriptor Interface(
        string fullName,
        string assemblyName,
        IEnumerable<string>? referencedTypeAssemblies = null,
        IEnumerable<string>? referencedTypeNames = null,
        IEnumerable<string>? implementedInterfaces = null)
    {
        return new StructuralTypeDescriptor(
            fullName,
            assemblyName,
            StructuralTypeKind.Interface,
            publicSettableProperties: null,
            referencedTypeAssemblies,
            referencedTypeNames,
            implementedInterfaces);
    }

    internal static StructuralTypeDescriptor FromType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var referencedTypes = new HashSet<Type>();
        AddType(referencedTypes, type.BaseType);

        foreach (Type implementedInterface in type.GetInterfaces())
        {
            AddType(referencedTypes, implementedInterface);
        }

        foreach (System.Reflection.PropertyInfo property in
            type.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.DeclaredOnly))
        {
            AddType(referencedTypes, property.PropertyType);
        }

        foreach (System.Reflection.FieldInfo field in type.GetFields(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.DeclaredOnly))
        {
            AddType(referencedTypes, field.FieldType);
        }

        foreach (System.Reflection.ConstructorInfo constructor in
            type.GetConstructors(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly))
        {
            foreach (System.Reflection.ParameterInfo parameter in
                constructor.GetParameters())
            {
                AddType(referencedTypes, parameter.ParameterType);
            }
        }

        foreach (System.Reflection.MethodInfo method in type.GetMethods(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.DeclaredOnly))
        {
            AddType(referencedTypes, method.ReturnType);

            foreach (System.Reflection.ParameterInfo parameter in
                method.GetParameters())
            {
                AddType(referencedTypes, parameter.ParameterType);
            }
        }

        string[] publicSettableProperties = type.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance)
            .Where(property =>
                property.SetMethod?.IsPublic == true &&
                !IsInitOnly(property) &&
                property.GetIndexParameters().Length == 0)
            .Select(property => property.Name)
            .ToArray();
        string[] referencedTypeNames = referencedTypes
            .Select(reference => reference.FullName)
            .Where(name => name is not null)
            .Select(name => name!)
            .ToArray();
        string[] referencedTypeAssemblies = referencedTypes
            .Select(reference =>
                reference.Assembly.GetName().Name)
            .Where(name => name is not null)
            .Select(name => name!)
            .ToArray();
        string[] implementedInterfaces = type.GetInterfaces()
            .Select(implementedInterface => implementedInterface.FullName)
            .Where(name => name is not null)
            .Select(name => name!)
            .ToArray();

        return new StructuralTypeDescriptor(
            type.FullName ?? type.Name,
            type.Assembly.GetName().Name ??
                throw new InvalidOperationException(
                    $"Assembly for '{type}' has no name."),
            GetKind(type),
            publicSettableProperties,
            referencedTypeAssemblies,
            referencedTypeNames,
            implementedInterfaces);
    }

    private static StructuralTypeKind GetKind(Type type)
    {
        if (type.IsInterface)
        {
            return StructuralTypeKind.Interface;
        }

        if (type.IsEnum)
        {
            return StructuralTypeKind.Enum;
        }

        return type.GetMethod(
            "<Clone>$",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance) is not null
            ? StructuralTypeKind.Record
            : StructuralTypeKind.Class;
    }

    private static bool IsInitOnly(
        System.Reflection.PropertyInfo property)
    {
        System.Reflection.MethodInfo? setter = property.SetMethod;

        return setter is not null &&
            setter.ReturnParameter
                .GetRequiredCustomModifiers()
                .Contains(
                    typeof(System.Runtime.CompilerServices.IsExternalInit));
    }

    private static void AddType(ISet<Type> types, Type? type)
    {
        if (type is null)
        {
            return;
        }

        if (type.HasElementType)
        {
            AddType(types, type.GetElementType());
        }

        if (type.IsGenericType)
        {
            foreach (Type argument in type.GetGenericArguments())
            {
                AddType(types, argument);
            }

            type = type.GetGenericTypeDefinition();
        }

        types.Add(type);
    }

    private static IReadOnlyList<string> Normalize(
        IEnumerable<string>? values)
    {
        return (values ?? [])
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private static string RequireValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Structural metadata value must not be empty.",
                parameterName);
        }

        return value;
    }
}
