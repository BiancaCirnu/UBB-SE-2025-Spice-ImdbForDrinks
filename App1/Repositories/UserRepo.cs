﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using App1.Models;
using Windows.System;

namespace App1.Repositories
{
    /// <summary>
    /// Repository for managing user data and operations.
    /// </summary>
    public class UserRepo : IUserRepository
    {
        /// <summary>
        /// Internal list of users managed by the repository.
        /// </summary>
        private readonly List<User> _usersList;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepo"/> class with default user data.
        /// </summary>
        public UserRepo()
        {
            List<Role> basicUserRoles = new List<Role>
            {
                new Role(RoleType.User, "user")
            };
            List<Role> adminRoles = new List<Role>
            {
                new Role(RoleType.User, "user"),
                new Role(RoleType.Admin, "admin")
            };
            List<Role> managerRoles = new List<Role>
            {
                new Role(RoleType.User, "user"),
                new Role(RoleType.Admin, "admin"),
                new Role(RoleType.Manager, "manager")
            };
            List<Role> bannedUserRoles = new List<Role>
            {
                new Role(RoleType.Banned, "banned")
            };
            _usersList = new List<User>
            {
                new User(
                    userId: 1,
                    emailAddress: "bianca.georgiana.cirnu@gmail.com",
                    fullName: "Bianca Georgiana Cirnu",
                    numberOfDeletedReviews: 2,
                    HasSubmittedAppeal: true,
                    assignedRoles: basicUserRoles
                ),
                new User(
                    userId: 3,
                    emailAddress: "admin.one@example.com",
                    fullName: "Admin One",
                    numberOfDeletedReviews: 0,
                    HasSubmittedAppeal: false,
                    assignedRoles: adminRoles
                ),
                new User(
                    userId: 5,
                    emailAddress: "admin.two@example.com",
                    fullName: "Admin Two",
                    numberOfDeletedReviews: 0,
                    HasSubmittedAppeal: false,
                    assignedRoles: adminRoles
                )
            };
        }

        /// <summary>
        /// Retrieves all users who have submitted appeals.
        /// </summary>
        /// <returns>A list of users who have submitted appeals.</returns>
        /// <exception cref="RepositoryException">Thrown when an error occurs while retrieving users.</exception>
        public List<User> GetUsersWhoHaveSubmittedAppeals()
        {
            try
            {
                if (_usersList == null)
                {
                    throw new NullReferenceException("_usersList is null.");
                }
                return _usersList.Where(user => user.HasSubmittedAppeal).ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve users who have submitted appeals.", ex);
            }
        }

        /// <summary>
        /// Retrieves all users with a specific role type.
        /// </summary>
        /// <param name="roleType">The role type to filter users by.</param>
        /// <returns>A list of users with the specified role type.</returns>
        /// <exception cref="RepositoryException">Thrown when an error occurs while retrieving users.</exception>
        public List<User> GetUsersByRoleType(RoleType roleType)
        {
            try
            {
                if (_usersList == null)
                {
                    throw new NullReferenceException("_usersList is null.");
                }
                return _usersList.Where(user => user.AssignedRoles != null && user.AssignedRoles.Any(role => role.RoleType == roleType)).ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve users with role type '{roleType}'.", ex);
            }
        }

        /// <summary>
        /// Retrieves the highest role type assigned to a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The highest role type assigned to the user.</returns>
        /// <exception cref="RepositoryException">Thrown when the user has no roles or does not exist.</exception>
        public RoleType GetHighestRoleTypeForUser(int userId)
        {
            var user = GetUserByID(userId);
            if (user.AssignedRoles == null || !user.AssignedRoles.Any())
            {
                throw new RepositoryException("User has no roles assigned.", new ArgumentException($"No roles found for user with ID {userId}"));
            }

            return user.AssignedRoles.Max(role => role.RoleType);
        }



        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <exception cref="RepositoryException">Thrown when the user does not exist or an error occurs.</exception>
        public User GetUserByID(int userId)
        {
            try
            {
                User? user = _usersList.FirstOrDefault(user => user.UserId == userId);
                if (user == null)
                {
                    throw new ArgumentException($"No user found with ID {userId}");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve user with ID {userId}.", ex);
            }
        }

        /// <summary>
        /// Retrieves all banned users who have submitted appeals.
        /// </summary>
        /// <returns>A list of banned users who have submitted appeals.</returns>
        /// <exception cref="RepositoryException">Thrown when an error occurs while retrieving users.</exception>
        public List<User> GetBannedUsersWhoHaveSubmittedAppeals()
        {
            try
            {
                if (_usersList == null)
                {
                    throw new NullReferenceException("_usersList is null.");
                }
                return _usersList.Where(user => user.HasSubmittedAppeal && user.AssignedRoles != null && user.AssignedRoles.Any(role => role.RoleType == RoleType.Banned)).ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve banned users who have submitted appeals.", ex);
            }
        }

        /// <summary>
        /// Adds a role to a user.
        /// </summary>
        /// <param name="userId">The ID of the user to add the role to.</param>
        /// <param name="roleToAdd">The role to add to the user.</param>
        /// <exception cref="RepositoryException">Thrown when the user does not exist or an error occurs.</exception>
        public void AddRoleToUser(int userId, Role roleToAdd)
        {
            try
            {
                User? user = _usersList.FirstOrDefault(user => user.UserId == userId);
                if (user == null)
                {
                    throw new ArgumentException($"No user found with ID {userId}");
                }

                user.AssignedRoles.Add(roleToAdd);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to add role to user with ID {userId}.", ex);
            }
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        /// <exception cref="RepositoryException">Thrown when an error occurs while retrieving users.</exception>
        public List<User> GetAllUsers()
        {
            try
            {
                if (_usersList == null)
                {
                    throw new NullReferenceException("_usersList is null.");
                }
                return _usersList;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve all users.", ex);
            }
        }

        /// <summary>
        /// Exception class for repository-related errors.
        /// </summary>
        public class RepositoryException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RepositoryException"/> class.
            /// </summary>
            /// <param name="message">The error message.</param>
            /// <param name="innerException">The inner exception.</param>
            public RepositoryException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }
    }
}