using System;
using System.Collections.Generic;
using System.Linq;
using App1.Models;
using App1.Repositories;
using Windows.System;
using static App1.Repositories.UserRepo;

namespace App1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _userRepository.GetAllUsers();
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve all users.", ex);
            }
        }

        public List<User> GetActiveUsersByRoleType(RoleType roleType)
        {
            try
            {
                return roleType switch
                {
                    > 0 => _userRepository.GetUsersByRoleType(roleType),
                    _ => throw new ArgumentException("Permission ID must be positive")
                };
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to get active users", ex);
            }
        }

        public List<User> GetBannedUsers()
        {
            try
            {
                return _userRepository.GetUsersByRoleType(RoleType.Banned);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to get banned users", ex);
            }
        }

        public List<User> GetUsersByRoleType(RoleType roleType)
        {
            try
            {
                return _userRepository.GetUsersByRoleType(roleType);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException($"Failed to retrieve users by role type '{roleType}'.", ex);
            }
        }
    
        public string GetUserFullNameById(int userId) 
        {
            try
            {
                var user = _userRepository.GetUserByID(userId);
                if (user == null)
                {
                    throw new UserServiceException($"Failed to retrieve the full name of the user with ID {userId}.", new ArgumentNullException(nameof(user)));
                }
                return user.FullName;
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException($"Failed to retrieve the full name of the user with ID {userId}.", ex);
            }
        }

        public List<User> GetBannedUsersWhoHaveSubmittedAppeals()
        {
            try
            {
                return _userRepository.GetBannedUsersWhoHaveSubmittedAppeals();
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve banned users who have submitted appeals.", ex);
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
                var user = _userRepository.GetUserByID(userId);
                if (user == null)
                {
                    throw new UserServiceException($"Failed to retrieve user with ID {userId}.", new ArgumentNullException(nameof(user)));
                }
                return user;
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException($"Failed to retrieve user with ID {userId}.", ex);
            }
        }

        public RoleType GetHighestRoleTypeForUser(int userId)
        {
            try
            {
                return _userRepository.GetHighestRoleTypeForUser(userId);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException($"Failed to retrieve the highest role type for user with ID {userId}.", ex);
            }
        }

        public List<User> GetAdminUsers()
        {
            try
            {
                return _userRepository.GetUsersByRoleType(RoleType.Admin);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve admin users.", ex);
            }
        }

        public List<User> GetRegularUsers()
        {
            try
            {
                return _userRepository.GetUsersByRoleType(RoleType.User);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve regular users.", ex);
            }
        }

        public List<User> GetManagers()
        {
            return _userRepository.GetUsersByRoleType(RoleType.Manager);
        }

        public void UpdateUserRole(int userId, RoleType roleType)
        {
            try
            {
                var user = _userRepository.GetUserByID(userId);
                if (user == null)
                    return;

                if (roleType == RoleType.Banned)
                {
                    bool hasBannedRole = false;
                    foreach (var role in user.AssignedRoles)
                    {
                        if (role.RoleType == RoleType.Banned)
                        {
                            hasBannedRole = true;
                            break;
                        }
                    }

                    if (!hasBannedRole)
                    {
                        user.AssignedRoles.Clear();
                        _userRepository.AddRoleToUser(userId, new Role(RoleType.Banned, "Banned"));
                    }
                }
                else
                {
                    user.AssignedRoles.Clear();
                    _userRepository.AddRoleToUser(userId, new Role(RoleType.User, "User"));
                }
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Failed to update user role", ex);
            }
        }
    }


    public class UserServiceException : Exception
    {
        public UserServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}