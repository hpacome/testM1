using ASPA.BOL.Dto;
using ASPA.DAL.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASPA.BOL.Mapping
{
    public class ApplicationUserMapping
    {
        public static User toEntity(ApplicationUserDto userDto)
        {
            return new User()
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.UserName
            };
        }

        public static ApplicationUserDto toDto(User user)
        {
            return new ApplicationUserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName
            };
        }
    }
}
