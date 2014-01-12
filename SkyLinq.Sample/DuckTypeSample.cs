using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyLinq.Composition;

namespace SkyLinq.Sample
{
    //Note that the Duck class does not implement any interface
    public class Duck
    {
        private string _name;

        public string Name
        {
            get { return this._name; }
            set 
            { 
                this._name = value;
                Console.WriteLine(string.Format("Duck name set to {0}.", value));
            }
        }

        public void Quack(int times)
        {
            Console.WriteLine(string.Format("Duck quacked {0} times.", times));
        }

        public double Walk()
        {
            double yards = 20.1;
            return yards;
        }
    }

    //That is how the caller expect from a duck
    public interface IDuckExpected
    {
        string Name { get; set; }

        void Quack(int times);
        double Walk();
    }

    public class DuckTypeSample : ISample
    {
        public void Run()
        {
            Duck duck = new Duck();
            DuckTypeProxyFactory factory = new DuckTypeProxyFactory();
            IDuckExpected proxy = factory.GenerateProxy<IDuckExpected>(typeof(IDuckExpected), duck);
            //IDuckExpected proxy = new DuckProxyExample(duck);
            //Calling proxy
            proxy.Name = "Oregon";
            Console.WriteLine(string.Format("Duck name is {0}.", proxy.Name));
            proxy.Quack(3);
            double yards = proxy.Walk();
            Console.WriteLine(string.Format("Duck walked {0} yards.", yards));
        }
    }

    //Should generate code like this one
    public class DuckProxyExample : IDuckExpected
    {
        Duck target;
        public DuckProxyExample(Duck duck)
        {
            target = duck;
        }

        public void Quack(int times)
        {
            target.Quack(times);
        }

        public double Walk()
        {
            return target.Walk();
        }

        public string Name
        {
            get { return target.Name; }
            set { target.Name = value; }
        }
    }
}
