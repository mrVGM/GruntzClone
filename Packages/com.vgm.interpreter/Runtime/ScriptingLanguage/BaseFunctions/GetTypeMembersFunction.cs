using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    public class GetTypeMembersFunction : IFunction
    {
        const string typeNameParam = "target_type";
        string[] args = { typeNameParam };
        public Scope ScopeTemplate 
        {
            get 
            {
                var scope = new Scope();
                scope.AddVariable(typeNameParam, null);
                return scope;
            }
        }

        public string[] ParameterNames => args;

        public object Execute(Scope scope)
        {
            var arg = scope.GetVariable(typeNameParam);
            var type = arg.GetType();
            if (arg is string) {
                type = Utils.GetTypeAcrossAssemblies(arg as string);
            }
            
            IEnumerable<MemberInfo> getMemberInfos<T>() where T : MemberInfo
            {
                var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                var curType = type;
                var res = new List<MemberInfo>();
                while (curType != null) {
                    res.AddRange(curType.GetMembers(flags).OfType<T>());
                    curType = curType.BaseType;
                }

                return res.Distinct();
            }

            GenericObject members = new GenericObject();
            var properties = members.GetPropoerty("properties");
            var methods = members.GetPropoerty("methods");
            var events = members.GetPropoerty("events");

            var allProperties = new List<MemberInfo>();
            allProperties.AddRange(getMemberInfos<PropertyInfo>());
            allProperties.AddRange(getMemberInfos<FieldInfo>());

            properties.ObjectValue = allProperties;
            methods.ObjectValue = getMemberInfos<MethodInfo>().ToList();
            events.ObjectValue = getMemberInfos<EventInfo>().ToList();
            
            return members;
        }
    }
}
