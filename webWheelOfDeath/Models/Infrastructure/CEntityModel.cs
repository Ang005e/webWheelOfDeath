using System.Security.AccessControl;
using LibWheelOfDeath;
using LibEntity;
using System.Configuration;

namespace webWheelOfDeath.Models.Infrastructure
{

    /// <summary>
    /// Base class for web models that map to LibWheelOfDeath entities.
    /// Enforces proper Id handling and consistent CRUD operations.
    /// Developed after constant troubles mapping and reading 
    /// entities from the backend's CRUDS library.
    /// 
    /// <typeparamref name="TEntity"/>The type of the CRUDS library class that this model mirrors.
    /// 
    /// </summary>
    public abstract class CEntityModel<TEntity> where TEntity : CEntity, new()
    {
        private TEntity _entity { get; set; } = new();

        /// <summary>
        /// The underlying entity's Id. This ensures Id is ALWAYS properly synchronised.
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
            // Don't map from empty entity - it may access uninitialised properties
            // Derived classes should set properties directly
        }

        /// <summary>
        /// Load constructor - MUST set Id and load the entity
        /// </summary>
        protected CEntityModel(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0 for loading an existing entity");

            _entity = new TEntity();
            Id = id;  // Set BEFORE Read()
            _entity.Read();   // Now Read() will work.
                              // I'm not relying on my implementations here... something is always wrong...
            MapFromEntity(_entity);
        }

        /// <summary>
        /// Must be correctly implemented;
        /// All enum operation types (create, read update, delete, search) must be accounted for.
        /// For example, isActive may have a default value of true, and that's acceptable for account creations. 
        /// However, it must be set for update opetations.
        /// <returns>a string of invalid properties, if any. Otherwise, an empty string.</returns>
        /// </summary>
        protected abstract void ValidateRequiredFields(bool isUpdate);

        /// <summary>
        /// Maps properties FROM the entity TO this model.
        /// Must be implemented by derived classes.
        /// ID should not be syncd.
        /// </summary>
        protected abstract void MapFromEntity(TEntity entity);

        /// <summary>
        /// Maps properties FROM this model TO the entity.
        /// Must be implemented by derived classes.
        /// ID should not be syncd.
        /// </summary>
        protected abstract void MapToEntity(TEntity entity);

        /// <summary>
        /// Creates a new entity in the database
        /// </summary>
        public virtual void Create()
        {
            if (!IsNew)
                throw new InvalidOperationException("Cannot create an entity that already has an Id");

            ValidateRequiredFields(false);

            MapToEntity(_entity);
            _entity.Create();
        }

        /// <summary>
        /// Updates an existing entity in the database
        /// </summary>
        public virtual void Update()
        {
            if (IsNew)
                throw new InvalidOperationException("Cannot update an entity without an Id");

            ValidateRequiredFields(false);

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
            var entity = new TEntity { Id = Id };
            MapToEntity(entity);
            return entity;
        }
    }
}