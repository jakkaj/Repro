using Autofac;
using Foundation;
using ReproTests.Model;
using ReproTests.Model.XamlingCore.iOS.Implementations;
using XamlingCore.Portable.Contract.Entities;
using XamlingCore.Portable.Data.Entities;
using XamlingCore.Portable.Data.Serialise;


namespace ReproTests.Glue
{
    public class ProjectGlue : GlueBase
    {
        public override void Init()
        {
            base.Init();

            Builder.RegisterType<LocalStorage>().As<ILocalStorage>().SingleInstance();


            Builder.RegisterType<LocalStorageFileRepo>().As<ILocalStorageFileRepo>().SingleInstance();

            Builder.RegisterGeneric(typeof(EntityManager<>)).As(typeof(IEntityManager<>)).SingleInstance();
            Builder.RegisterGeneric(typeof(EntityBucket<>)).As(typeof(IEntityBucket<>)).SingleInstance();
            Builder.RegisterType<JsonNETEntitySerialiser>().As<IEntitySerialiser>().SingleInstance();
            Builder.RegisterType<EntityCache>().As<IEntityCache>().SingleInstance();
            Container = Builder.Build();

        }
    }

    public abstract class GlueBase : IGlue
    {
        protected ContainerBuilder Builder;

        public virtual void Init()
        {
            Builder = new ContainerBuilder();
        }

        public IContainer Container { get; protected set; }
    }

    public interface IGlue
    {
        void Init();
        IContainer Container { get; }
    }
}