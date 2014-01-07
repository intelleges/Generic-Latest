using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Generic.Models;

namespace Generic.DataLayer
{
    public class MenuOperation
    {
        EntitiesDBContext db = new EntitiesDBContext();

        #region Menu  DBOperation

        public IEnumerable<MenuModel> GetAllParentMenu()
        {
            IEnumerable<MenuModel> menuModel = (from t in db.menu
                                                   where t.parentid == t.id
                                                   select new MenuModel
                                                   {
                                                       id = t.id,
                                                       parentid = t.parentid,
                                                       url = t.url,
                                                       sortOrder = t.sortOrder,
                                                       description = t.description,
                                                       accesslevel = t.accesslevel,
                                                       active = t.active,
                                                       enterprise = t.enterprise

                                                   }).AsEnumerable<MenuModel>();
            return menuModel;
        }

        public List<MenuModel> GetMenus(int? parentId, int menuId)
        {
            List<MenuModel> menuModel = (from t in db.menu
                                            where t.parentid != t.id && t.parentid == parentId
                                            select new MenuModel
                                            {
                                                id = t.id,
                                                parentid = t.parentid,
                                                url = t.url,
                                                sortOrder = t.sortOrder,
                                                description = t.description,
                                                accesslevel = t.accesslevel,
                                                active = t.active,
                                                enterprise = t.enterprise

                                            }).ToList();
            return menuModel;
        }

        #endregion
    }
}