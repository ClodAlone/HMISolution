using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace AutoNotifyPropertyChange
{
    static class PropertyInfoExtension
    {
        public static bool IsVirtual(this PropertyInfo pi)
        {
            return (pi.CanRead == false || pi.GetGetMethod().IsVirtual)
                    &&
                    (pi.CanWrite == false || pi.GetSetMethod().IsVirtual);
        }
    }
    static class ProxyGen
    {
        static object lockobject = new object();
        static Dictionary<Type, Type> proxies = new Dictionary<Type, Type>();
        internal const string PropertyChangedFunctionName = "OnPropertyChanged";
        internal const string PropertyChangedEventName = "PropertyChanged";
        private const string PropertyNameParameterName = "propertyName";
        
        public static Type GetFor(Type t, Dictionary<string, List<string>> influences)
        {
            if (HasFor(t))
                return For(t);
            Store(CreateFor(t, influences), t);
            return For(t);
        }

        private static void Store(Type type,Type key)
        {
            lock (lockobject)
            {
                proxies[key] = type;
            }
        }

        private static Type CreateFor(Type t,Dictionary<string,List<string>> influences)
        {
            var provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateInMemory = true;
            CodeCompileUnit cu = new CodeCompileUnit();
            AddAllAssemblyAsReference(cu);
            cu.Namespaces.Add(CreateNamespace(t,influences));
#if DEBUG
            StringWriter sw = new StringWriter();
            provider.GenerateCodeFromCompileUnit(cu,sw , new CodeGeneratorOptions() { BracingStyle="C" });
            Trace.WriteLine(sw.GetStringBuilder());
#endif
            CompilerResults cr = provider.CompileAssemblyFromDom(cp,cu);
            if (cr.Errors.Count > 0)
            {
                ThrowErrors(cr.Errors);
            }
            return cr.CompiledAssembly.GetTypes()[0];
        }

        private static void AddAllAssemblyAsReference(CodeCompileUnit cu)
        {
            foreach (var v in AppDomain.CurrentDomain.GetAssemblies())
            {
                cu.ReferencedAssemblies.Add(v.Location);
            }
        }

        private static void ThrowErrors(CompilerErrorCollection compilerErrorCollection)
        {
            StringBuilder sb = new StringBuilder();
            foreach( CompilerError  e in compilerErrorCollection)
            {
                sb.AppendLine(e.ErrorText);
            }
            throw new ProxyGenerationException("Compiler errors:\n" + sb.ToString());
        }

        private static CodeNamespace CreateNamespace(Type t,Dictionary<string,List<string>> influences)
        {
            var nsp = new CodeNamespace("__autonotifypropertychanged");
            var decl = new CodeTypeDeclaration();
            decl.Name = GetNameForDerivedClass(t);
            decl.TypeAttributes = TypeAttributes.NotPublic;
            decl.Attributes = MemberAttributes.Private;
            decl.BaseTypes.Add(t);
            if (!t.GetInterfaces().Any(I => I == typeof(INotifyPropertyChanged)))
            {
                ImplementINotifyPropertyChanged(decl);
            }
            foreach (var ci in t.GetConstructors())
            {
                if (ci.IsPublic && ci.GetParameters().Length > 0)
                {
                    AddDerivedConstructor(decl,ci);
                }
            }
            foreach (PropertyInfo pi in t.GetProperties().Where(p =>p.IsVirtual())) 
            {
                // make sense only to override properties having setter and getter
                if (pi.CanWrite && pi.CanRead)
                {
                    List<string> additional = new List<string>();
                    if (influences.ContainsKey(pi.Name))
                        additional = influences[pi.Name];
                    decl.Members.Add(CreatePropertyOverride(pi,additional));
                }
            }
            nsp.Types.Add(decl);
            return nsp;
        }

        private static void ImplementINotifyPropertyChanged(CodeTypeDeclaration decl)
        {
            decl.BaseTypes.Add(typeof(INotifyPropertyChanged));
            decl.Members.Add(new CodeMemberEvent() 
                            { Name=PropertyChangedEventName
                             , Attributes=MemberAttributes.Public
                             , Type = new CodeTypeReference(typeof(PropertyChangedEventHandler))
                            });
            var notify = new CodeMemberMethod()
                            {
                                Name = PropertyChangedFunctionName
                                ,
                                Attributes = MemberAttributes.Family

                            };
            decl.Members.Add(notify);
            notify.Parameters.Add(new CodeParameterDeclarationExpression() { Name=PropertyNameParameterName,Type=new CodeTypeReference(typeof(string)) });
            var condition = new CodeConditionStatement()
            {
                Condition = new CodeBinaryOperatorExpression()
              {
                  Left = new CodePrimitiveExpression(null)
                  ,
                  Right = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), PropertyChangedEventName)
                  ,
                  Operator = CodeBinaryOperatorType.IdentityInequality
              }
            };
            var eventArgs = new CodeObjectCreateExpression() { CreateType=new CodeTypeReference(typeof(PropertyChangedEventArgs)) };
            eventArgs.Parameters.Add(new CodeVariableReferenceExpression(PropertyNameParameterName));

            var invoke = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), PropertyChangedEventName);
            invoke.Parameters.Add(new CodeThisReferenceExpression());
            invoke.Parameters.Add(eventArgs);
            condition.TrueStatements.Add(
                    invoke
                );
            
            notify.Statements.Add(condition);
        }

        private static void AddDerivedConstructor(CodeTypeDeclaration decl, ConstructorInfo ci)
        {
            CodeConstructor cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;
            foreach(var pi in ci.GetParameters() )
            {
                cc.Parameters.Add(new CodeParameterDeclarationExpression() { Name=pi.Name, Type=new CodeTypeReference(pi.ParameterType), Direction=ToDirection(pi) });
                cc.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(pi.Name));
            }
            
            decl.Members.Add(cc);
        }

        private static FieldDirection ToDirection(ParameterInfo pi)
        {
            if (pi.IsIn)
                return FieldDirection.In;
            if (pi.IsOut)
                return FieldDirection.Out;
            if (pi.ParameterType.IsByRef)
                return FieldDirection.Ref;
            return FieldDirection.In;
        }

        

        private static CodeTypeMember CreatePropertyOverride(PropertyInfo pi,List<string> additionals)
        {
            CodeMemberProperty mp = new CodeMemberProperty();
            mp.Name = pi.Name;
            mp.Attributes = MemberAttributes.Override|MemberAttributes.Public;
            mp.Type = new CodeTypeReference(pi.PropertyType);
            mp.HasGet = mp.HasSet = true;
            
            mp.GetStatements.Add( new CodeMethodReturnStatement( new CodePropertyReferenceExpression() { PropertyName = pi.Name, TargetObject = new CodeBaseReferenceExpression() }));

            var conditionForNotify = new CodeMethodInvokeExpression() { Method = new CodeMethodReferenceExpression(new CodePropertyReferenceExpression() { PropertyName = pi.Name, TargetObject = new CodeBaseReferenceExpression() }, "Equals") };
            conditionForNotify.Parameters.Add(new CodePropertySetValueReferenceExpression());
            var setCondition = new CodeConditionStatement()
                                {
                                    Condition = new CodeBinaryOperatorExpression() { Left = new CodePrimitiveExpression(false), Right = conditionForNotify, Operator = CodeBinaryOperatorType.IdentityEquality }

                                };
            setCondition.TrueStatements.Add(new CodeAssignStatement() { Left = new CodePropertyReferenceExpression() { PropertyName = pi.Name, TargetObject = new CodeBaseReferenceExpression() }, Right = new CodePropertySetValueReferenceExpression() });
            
            
            var invokeMethod = new CodeMethodInvokeExpression() { Method = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), PropertyChangedFunctionName) };
            invokeMethod.Parameters.Add(new CodePrimitiveExpression(pi.Name));
            setCondition.TrueStatements.Add(invokeMethod);

            foreach (var additional in additionals)
            {
                invokeMethod = new CodeMethodInvokeExpression() { Method = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), PropertyChangedFunctionName) };
                invokeMethod.Parameters.Add(new CodePrimitiveExpression(additional));
                setCondition.TrueStatements.Add(invokeMethod);
            }

            mp.SetStatements.Add(setCondition);
            return mp;
        }

       

        private static string GetNameForDerivedClass(Type t)
        {
            return string.Concat("__autonotify", t.Name);
        }

        private static bool HasFor(Type t)
        {
            lock (lockobject)
            {
                return proxies.ContainsKey(t);
            }
        }

        private static Type For(Type t)
        {
            lock (lockobject)
            {
                return proxies[t];
            }
        }

       
    }
}
