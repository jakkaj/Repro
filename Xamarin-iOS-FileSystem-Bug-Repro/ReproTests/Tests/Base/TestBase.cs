
using Autofac;
using ReproTests.Glue;

namespace ReproTests.Tests.Base
{
    public class TestBase
    {
        protected IContainer Container;
        public TestBase()
        {
            var glue = new ProjectGlue();
            glue.Init();

            Container = glue.Container;
        }

        public T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }
    }
}