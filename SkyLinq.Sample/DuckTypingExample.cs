using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyLinq.Composition;

namespace SkyLinq.Example
{
    //Note that the Other Duck class does not implement any interface
    public class OtherDuck
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
    public interface IMyDuck
    {
        string Name { get; set; }

        void Quack(int times);
        double Walk();
    }

    public class DuckTypingExample : IExample
    {
        public void Run()
        {
            OtherDuck duck = new OtherDuck();
            DuckTypingProxyFactory factory = new DuckTypingProxyFactory();
            IMyDuck proxy = factory.GenerateProxy<IMyDuck>(duck);
            //IMyDuck proxy = new DuckProxyExample(duck);

            //Calling proxy
            proxy.Name = "Oregon";
            Console.WriteLine(string.Format("Duck name is {0}.", proxy.Name));
            proxy.Quack(3);
            double yards = proxy.Walk();
            Console.WriteLine(string.Format("Duck walked {0} yards.", yards));
        }
    }

    //The DuckTypingProxyFactory should generate a class exactly like the following
    public class DuckProxyExample : IMyDuck
    {
        OtherDuck target;
        public DuckProxyExample(OtherDuck duck)
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
