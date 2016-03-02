namespace LVMS.FactuurSturen.Model
{
    public enum RepeatTypes
    {
        None,
        /// <summary>
        /// Send automatically when due
        /// </summary>
        Auto,
        /// <summary>
        /// Do not send automatically
        /// </summary>
        Manual
    }
}