using Doan.Models.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan.Models.MD
{
    public class LoginMD
    {

        Db_Doan db = null;
        public LoginMD()
        {
            db = new Db_Doan();
        }
        public User GetById(string userName)
        {
            return db.Users.SingleOrDefault(x => x.UserName == userName);
        }
        public List<string> GetListCredential(string userName)
        {
            var user = db.Users.Single(x => x.UserName == userName);
            var data = (from a in db.Credentials
                        join b in db.UserGroups on a.UserGroupID equals b.Id
                        join c in db.Roles on a.RoleId equals c.ID
                        where b.Id == user.GroupID
                        select new
                        {
                            RoleID = a.RoleId,
                            UserGroupID = a.UserGroupID
                        }).AsEnumerable().Select(x => new Credential()
                        {
                            RoleId = x.RoleID,
                            UserGroupID = x.UserGroupID
                        });
            return data.Select(x => x.RoleId).ToList();

        }
        public int Login(string userName, string passWord, bool isLoginAdmin = false)
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == userName);
            if (result == null)
            {
                return 0;
            }
            else
            {
                if (isLoginAdmin == true)
                {
                    if (result.GroupID == CommonConstants.ADMIN_GROUP || result.GroupID == CommonConstants.MOD_GROUP)
                    {
                        if (result.Status == false)
                        {
                            return -1;
                        }
                        else
                        {
                            if (result.Password == passWord)
                                return 1;
                            else
                                return -2;
                        }
                    }
                    else if (result.GroupID == CommonConstants.MEMBER_GROUP)
                    {
                        if (result.Status == false)
                        {
                            return -1;
                        }
                        else
                        {
                            if (result.Password == passWord)
                                return 4;
                            else
                                return -2;
                        }
                    }
                    else
                    {
                        return -3;
                    }
                }
                else
                {
                    if (result.Password == passWord)
                        return 1;
                    else
                        return -2;
                }
            }
        }
        public long InsertForFacebook(Customer entity)
        {
            var user = db.Customers.SingleOrDefault(x => x.Email_Cus == entity.Email_Cus);
            if (user == null)
            {
                db.Customers.Add(entity);
                db.SaveChanges();
                return entity.CodeCus;
            }
            else
            {
                return user.CodeCus;
            }

        }
    }
}