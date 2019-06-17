using DAO;
using Domain.Common;
using Model.Entities;
using System;

namespace Domain
{
    /// <summary>
    /// Users：服务访问对象
    /// </summary>
    public class UsersService : ServiceBaseExtension<UsersEntity>
    {
        private readonly UsersRepository _UsersRepository;
        private UserInfoService _userInfoService;

        public String Str = Guid.NewGuid().ToString("N");

        public UsersService(UsersRepository usersRepository, UserInfoService userInfoService)
        {
            _UsersRepository = usersRepository;
            _userInfoService = userInfoService;

            //UsersEntity u = new UsersEntity();
            //UserInfoEntity ui = new UserInfoEntity();
            //using (HY.DataAccess.Transactions.TransactionScope tranScope = new HY.DataAccess.Transactions.TransactionScope())
            //{
            //    this.Insert(u);
            //    _userInfoService.Insert(ui);
            //    tranScope.Complete();
            //}



            //IDbTransaction tran = this.DBSession.Begin();
            //this.Insert(u,tran);
            //_userInfoService.Insert(ui,tran);
            //tran.Commit();


        }


    }
}
