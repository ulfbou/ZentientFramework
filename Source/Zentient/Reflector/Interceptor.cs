using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;

namespace Zentient.Reflector
{
    public class Interceptor
    {
        public static T CreateProxy<T>(T target)
        {
            Type type = typeof(T);
            TypeBuilder typeBuilder = CreateTypeBuilder(type);

            ConstructorInfo constructor = CreateConstructor(typeBuilder, type);
            CreateInterceptedMethods(typeBuilder, type);

            Type proxyType = typeBuilder.CreateType();
            return (T)Activator.CreateInstance(proxyType, target);
        }

        public static TypeBuilder CreateTypeBuilder(Type type)
        {
            var assemblyName = new AssemblyName(type.Assembly.GetName().Name + "Proxy");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(type.Assembly.GetName().Name + "Proxy");
            var typeBuilder = moduleBuilder.DefineType(type.Name + "Proxy", TypeAttributes.Public, type);

            return typeBuilder;
        }

        private static ConstructorInfo CreateConstructor(TypeBuilder typeBuilder, Type targetType)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { targetType });
            constructorBuilder.DefineParameter(1, ParameterAttributes.None, "target");
            var fieldBuilder = typeBuilder.DefineField("target", targetType, FieldAttributes.Private);

            var il = constructorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return constructorBuilder;
        }

        private static void CreateInterceptedMethods(TypeBuilder typeBuilder, Type type)
        {
            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual);
                var methodIl = methodBuilder.GetILGenerator();

                methodIl.Emit(OpCodes.Ldstr, methodInfo.Name);
                methodIl.Emit(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string) }));

                methodIl.Emit(OpCodes.Ldarg_0);
                methodIl.Emit(OpCodes.Ldfld, typeBuilder.DefineField("target", type, FieldAttributes.Private));

                var parameters = methodInfo.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    methodIl.Emit(OpCodes.Ldarg, i + 1);
                }

                methodIl.Emit(OpCodes.Call, methodInfo);
                methodIl.Emit(OpCodes.Ret);
            }
        }

        public static void Inspect<T>() where T : class
        {
            var type = typeof(T);
            var access = InspectAccessibility(typeof(T));
            var typeName = type.Name;
            var baseTypeName = type.BaseType?.Name;


            Console.WriteLine($"{access} {typeName} : {baseTypeName}");

            // Get the type's attributes
            var attributes = type.GetMembers().Select(m => m.Name).ToList();

            // Get the type's methods
            var methods = type.GetMethods().Select(m => m.Name).ToList();

            // Get the type's properties
            var properties = type.GetProperties().Select(p => p.Name).ToList();

            // Get the type's fields (non-method attributes)
            var fields = type.GetFields().Select(f => f.Name).ToList();

            // Print the information
            Console.WriteLine($"Attributes: {string.Join(", ", attributes)}");
            Console.WriteLine($"Methods: {string.Join(", ", methods)}");
            Console.WriteLine($"Properties: {string.Join(", ", properties)}");
            Console.WriteLine($"Fields: {string.Join(", ", fields)}");
        }

        public static void Inspect(object obj)
        {
            Type type = obj.GetType();
            var ctors = type.GetConstructors();
            var methods = type.GetMethods();

            Console.WriteLine($"GetMethods: {methods.Count()}");
            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                var methodName = method.Name;
                var parameterTypes = InspectParameters(method.GetParameters());
                var param = string.Join(", ", parameterTypes);

                Console.WriteLine($"{returnType.Name} {method.Name}({param})");
            }
        }

        private static IEnumerable<string> InspectParameters(ParameterInfo[] parameterInfos)
        {
            foreach (var parameter in parameterInfos)
            {
                var pAtt = InspectAttributes(parameter.GetCustomAttributes(true));
                var pMod = parameter.GetModifiedParameterType();
                var pType = parameter.GetType().Name;
                var pName = parameter.Name;

                yield return $"{pAtt}{pMod} {pName}";
            }
        }

        private static string InspectAttributes(object[] objects)
        {
            List<string> attributes = new List<string>();

            foreach (var obj in objects)
            {
                var type = obj.GetType();
                var typeName = type.FullName;
                var isAttrib = type.IsClass && type.IsDefined(typeof(Attribute), false);
                attributes.Add($"[{(isAttrib ? "*" : "")}{typeName}] ");
            }

            return string.Join(", ", attributes);
        }

        public enum Accessibility { Public, Internal, Protected, Private }
        public struct AccessorInfo
        {
            Accessibility accessibility;

            public bool IsAbstract { get; internal set; }
            public bool ImplementsInterface { get; internal set; }
        }
        public static string InspectAccessibility(Type type)
        {
            TypeAttributes accessibility = type.Attributes & TypeAttributes.VisibilityMask;

            AccessorInfo accessorInfo = new();
            accessorInfo.IsAbstract = true;
            accessorInfo.ImplementsInterface = true;

            //TypeAttributes.Interface ac;
            if ((accessibility & TypeAttributes.Public) == TypeAttributes.Public)
            {
                return "public";
            }

            if ((accessibility & TypeAttributes.NestedPublic) == TypeAttributes.NestedPublic)
            {
                return "public nested";
            }

            if ((accessibility & TypeAttributes.NestedPrivate) == TypeAttributes.NestedPrivate)
            {
                return "private nested";
            }

            if ((accessibility & TypeAttributes.NestedFamily) == TypeAttributes.NestedFamily)
            {
                return "protected nested";
            }
            
            if ((accessibility & TypeAttributes.NestedAssembly) == TypeAttributes.NestedAssembly)
            {
                return "internal nested";
            }
            
            if ((accessibility & TypeAttributes.NestedFamANDAssem) == TypeAttributes.NestedFamANDAssem)
            {
                return "internal protected nested";
            }

            if ((accessibility & TypeAttributes.NestedFamORAssem) == TypeAttributes.NestedFamORAssem)
            {
                return "protected or internal nested";
            }

            return "unknown";
        }
    }

}