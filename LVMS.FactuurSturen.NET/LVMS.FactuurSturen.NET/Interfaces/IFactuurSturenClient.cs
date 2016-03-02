using System.Threading.Tasks;

namespace LVMS.FactuurSturen.Interfaces
{
    interface IFactuurSturenClient
    {
        /// <summary>
        /// Establishes a secure connection with the FactuurSturen.nl API service.
        /// It first calls user/init to get the nonce and then calls user/login
        /// to establish a secure session.
        /// </summary>
        /// <param name="userNameEmail">Username or E-mail address</param>
        /// <param name="password"></param>
        /// <returns><c>true</c> if success, <c>false</c> if authentication failed.</returns>
        Task LoginAsync(string userNameEmail, string password);
        Task<bool> CheckConnection();
    }
}
