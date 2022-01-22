using System;

namespace ScriptingLanguage.Interpreter
{
    public class Number
    {
        public static Type[] SupportedTypes = { typeof(int), typeof(float), typeof(double) };
        public double DoubleValue;

        public Number(double num) 
        {
            DoubleValue = num;
        }

        public object GetNumber(Type t) 
        {
            return Convert.ChangeType(DoubleValue, t);
        }

        public override string ToString()
        {
            return DoubleValue.ToString();
        }
    }
}
