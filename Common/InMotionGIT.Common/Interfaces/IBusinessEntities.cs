
namespace InMotionGIT.Common.Interfaces
{

    public interface IBusinessEntities
    {

        /// <summary>
        /// Gets or sets the object state is loaded from the database or not.
        /// </summary>
        /// <value>true if the object is loaded from database; otherwise, false.</value>
        bool IsNew { get; set; }

        /// <summary>
        /// Gets or sets if the object state is modified or not.
        /// </summary>
        /// <value>true if the object is modified; otherwise, false.</value>
        bool isDirty { get; set; }

        /// <summary>
        /// Gets or sets if the object state is pending for delete or not.
        /// </summary>
        /// <value>true if the object is delete masked; otherwise, false.</value>
        bool isDeletedMark { get; set; }

        /// <summary>
        /// Gets or sets the object that contains data about the object.
        /// </summary>
        /// <value>An Object that contains data about the object. The default is null reference (Nothing in Visual Basic).</value>
        /// <returns>System.Object</returns>
        object Tag { get; set; }

    }

}