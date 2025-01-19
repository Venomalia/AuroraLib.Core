namespace AuroraLib.Core.Format
{
    /// <summary>
    /// Represents main MIME type categories.
    /// </summary>
    public enum MIMEType
    {
        /// <summary>
        /// Any kind of binary data that doesn't fall explicitly into one of the other types.
        /// </summary>
        Application,

        /// <summary>
        /// Audio files and data.
        /// </summary>
        Audio,

        /// <summary>
        /// Image or graphical data including both bitmap and vector still images as well as animated versions of still image formats.
        /// </summary>
        Image,

        /// <summary>
        /// Message formats.
        /// </summary>
        Message,

        /// <summary>
        /// Multipart types indicate a category of document broken into pieces, often with different MIME types.
        /// </summary>
        Multipart,

        /// <summary>
        /// Human-readable content.
        /// </summary>
        Text,

        /// <summary>
        /// Font files.
        /// </summary>
        Font,

        /// <summary>
        /// Video data or files, such as MP4 movies.
        /// </summary>
        Video,

        /// <summary>
        /// Representing multidimensional structures like model data for a 3D object or scene.
        /// </summary>
        Model,
    }
}
