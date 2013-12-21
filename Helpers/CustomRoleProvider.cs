using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Generic.Helpers
{
    public class CustomRoleProvider : RoleProvider
    {
        #region Variables

        private hs3MVCMTQa2Entities db;

        #endregion

        #region Constructors

        public CustomRoleProvider()
        {
            this.db = new hs3MVCMTQa2Entities();
        }

        #endregion

        #region Implemented RoleProvider Methods

        public override bool IsUserInRole(string userName, string roleName)
        {
            pr_getRoleByName_Result role = db.pr_getRoleByName(roleName, Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            pr_getPersonByEmail_Result user = db.pr_getPersonByEmail(userName, Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            
            if (user == null)
                return false;
            if (role == null)
                return false;

            return user.Role == role.description;

        }

        public override string[] GetRolesForUser(string userName)
        {
            pr_getPersonByEmail_Result user = db.pr_getPersonByEmail(userName, Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            
            return new string[] { user.Role };
        }

        #endregion

        #region Not Implemented RoleProvider Methods

        #region Properties

        /// <summary>
        /// This property is not implemented.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        /// <summary>
        /// This function is not implemented.
        /// </summary>
        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function is not implemented.
        /// </summary>
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is not implemented.
        /// </summary>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function is not implemented.
        /// </summary>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is not implemented.
        /// </summary>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is not implemented.
        /// </summary>
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function is not implemented.
        /// </summary>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function is not implemented.
        /// </summary>
        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}