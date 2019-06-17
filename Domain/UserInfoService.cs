using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensions;
using Domain.Common;
using Model.Entities;
using XNY.Freamework;
using XNY.DataAccess;
using DAO;

namespace Domain
{
    /// <summary>
    /// UserInfo：服务访问对象
    /// </summary>
    public class UserInfoService : ServiceBaseExtension<UserInfoEntity>
    {
        private readonly UserInfoRepository _UserInfoRepository;

        public UserInfoService(UserInfoRepository userInfoRepository)
        {
            _UserInfoRepository = userInfoRepository;
        }


    }
}
