using System.Security.AccessControl;
using LibWheelOfDeath;
using LibEntity;
namespace webWheelOfDeath.Models
{
    using System.Security.AccessControl;
    using LibWheelOfDeath;
    using LibEntity;
    using global::webWheelOfDeath.Exceptions;

    namespace webWheelOfDeath.Models
    {
        /// <summary>
        /// Base class for web models that map to LibWheelOfDeath entities.
        /// Enforces proper Id handling and consistent CRUD operations.
        /// This was a critical patch introduced to prevent the countless subtle 
        /// errors that arose from inconsistent mapping within model classes.
        /// </summary>
        public abstract class CEntityModel<TEntity> where TEntity : CEntity, new()
        {
            private TEntity _entity { get; set; } = new();

            /// <summary>
            /// The underlying entity's Id. This ensures Id is always properly synchronized.
            /// </summary>
            public long Id
            {
                get => _entity.Id;
                set => _entity.Id = value;
            }

            /// <summary>
            /// Indicates if this is a new entity (Id == 0)
            /// </summary>
            public bool IsNew => Id == 0;

            /// <summary>
            /// Default constructor - creates a new entity
            /// </summary>
            protected CEntityModel()
            {
                _entity = new TEntity();
                MapFromEntity(_entity);
            }

            /// <summary>
            /// Load constructor - MUST set Id and load the entity
            /// </summary>
            protected CEntityModel(long id)
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be greater than 0 for loading an existing entity");

                _entity = new TEntity();
                _entity.Id = id;  // Set BEFORE Read()
                _entity.Read();   // Now Read() will work
                MapFromEntity(_entity);
            }

            /// <summary>
            /// Maps properties FROM the entity TO this model.
            /// Must be implemented by derived classes.
            /// </summary>
            protected abstract void MapFromEntity(TEntity entity);

            /// <summary>
            /// Maps properties FROM this model TO the entity.
            /// Must be implemented by derived classes.
            /// </summary>
            protected abstract void MapToEntity(TEntity entity);

            /// <summary>
            /// Creates a new entity in the database
            /// </summary>
            public virtual void Create()
            {
                if (!IsNew)
                    throw new InvalidOperationException("Cannot create an entity that already has an Id");

                MapToEntity(_entity);
                _entity.Create();
                Id = _entity.Id;  // Sync the generated Id back
            }

            /// <summary>
            /// Updates an existing entity in the database
            /// </summary>
            public virtual void Update()
            {
                if (IsNew)
                    throw new InvalidOperationException("Cannot update an entity without an Id");

                MapToEntity(_entity);
                _entity.Update();
            }

            /// <summary>
            /// Deletes the entity from the database
            /// </summary>
            public virtual void Delete()
            {
                if (IsNew)
                    throw new InvalidOperationException("Cannot delete an entity without an Id");

                _entity.Delete();
            }

            /// <summary>
            /// Reloads the entity from the database
            /// </summary>
            public virtual void Refresh()
            {
                if (IsNew)
                    throw new InvalidOperationException("Cannot refresh an entity without an Id");

                _entity.Read();
                MapFromEntity(_entity);
            }

            /// <summary>
            /// Builds a new entity instance with current model values
            /// </summary>
            public TEntity BuildEntity()
            {
                var entity = new TEntity { Id = this.Id };
                MapToEntity(entity);
                return entity;
            }

        }
    }
}
