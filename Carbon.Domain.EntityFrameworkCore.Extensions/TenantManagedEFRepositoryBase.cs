﻿using Carbon.Common;
using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.Common.TenantManagementHandler.Interfaces;
using Carbon.Domain.Abstractions.Entities;
using Carbon.Domain.Abstractions.Repositories;
using Carbon.ExceptionHandling.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore
{
    public abstract class TenantManagedEFRepositoryBase : TenantManagedRepositoryBase
    {
        private readonly DbContext TargetErDbContext;

        private DbSet<EntitySolutionRelation> TargetErDbSet;

        public TenantManagedEFRepositoryBase()
        {

        }
        public TenantManagedEFRepositoryBase(DbContext ctx)
        {
            TargetErDbContext = ctx;
            TargetErDbSet = TargetErDbContext.Set<EntitySolutionRelation>();
        }

        public override async Task ConnectToSolution<T>(T relatedEntity)
        {
            if (relatedEntity == null || relatedEntity.RelationalOwners == null || !relatedEntity.RelationalOwners.Any() || relatedEntity.RelationalOwners.Where(k => k == null).Any())
                return;
            bool relationsChanged = false;
            
            foreach (var ro in relatedEntity.RelationalOwners)
            {
                ro.EntityId = relatedEntity.Id;
                ro.EntityCode = relatedEntity.GetObjectTypeCode();
            }

            var oldRelations = await TargetErDbSet.Where(k => !k.IsDeleted && k.EntityCode == relatedEntity.GetObjectTypeCode() && k.EntityId == relatedEntity.Id).ToListAsync();

            if (oldRelations == null || !oldRelations.Any())
                relationsChanged = true;
            else
            {
                var firstSet = new HashSet<Guid>(oldRelations.Select(k => k.SolutionId).ToList());
                var secondSet = new HashSet<Guid>(relatedEntity.RelationalOwners.Select(k => k.SolutionId).ToList());

                var allEqual = secondSet.SetEquals(firstSet);
                if (!allEqual)
                {
                    relationsChanged = true;
                    TargetErDbSet.RemoveRange(oldRelations);
                    await TargetErDbContext.SaveChangesAsync();
                }
            }

            if (relationsChanged)
            {
                foreach (var ro in relatedEntity.RelationalOwners)
                {
                    ro.Id = Guid.NewGuid();
                }

                TargetErDbSet.AddRange(relatedEntity.RelationalOwners);
                await TargetErDbContext.SaveChangesAsync();
            }
        }

        public override async Task RemoveSolutions<T>(T relatedEntity)
        {
            if (relatedEntity == null)
                return;

            var entitySolutionRelations = await TargetErDbSet.Where(k => !k.IsDeleted && k.EntityCode == relatedEntity.GetObjectTypeCode() && k.EntityId == relatedEntity.Id).ToListAsync();
            TargetErDbSet.RemoveRange(entitySolutionRelations);
            await TargetErDbContext.SaveChangesAsync();
        }

        public override void CheckIfAuthorized<T>(T relatedEntity)
        {
            if(relatedEntity.OwnerType == OwnerType.None || relatedEntity.OwnerId == Guid.Empty)
            {
                return;
            }

            bool permitted = false;
            if (relatedEntity.OwnerType == OwnerType.CustomerBased)
            {
                permitted = true;
            }
            else
            {
                foreach (var permissionDetailedDto in FilterOwnershipList)
                {
                    if (permissionDetailedDto.PrivilegeLevelType == PermissionGroupImpactLevel.User)
                    {
                        permitted |= (relatedEntity.OwnerType == OwnerType.User || relatedEntity.OwnerType == OwnerType.UserGroup) && relatedEntity.OwnerId == permissionDetailedDto.UserId;
                    }
                    else if (permissionDetailedDto.PrivilegeLevelType == PermissionGroupImpactLevel.OnlyPolicyItself
                        || permissionDetailedDto.PrivilegeLevelType == PermissionGroupImpactLevel.PolicyItselfAndItsChildPolicies
                        || permissionDetailedDto.PrivilegeLevelType == PermissionGroupImpactLevel.AllPoliciesIncludedInZone)
                    {
                        permitted |= (((relatedEntity.OwnerType == OwnerType.User || relatedEntity.OwnerType == OwnerType.Organization) && permissionDetailedDto.Policies.Contains(relatedEntity.OrganizationId))
                            || (relatedEntity.OwnerType == OwnerType.Role && relatedEntity.OwnerId == permissionDetailedDto.RoleId));
                    }
                }
            }
            if(!permitted)
            {
                throw new ForbiddenOperationException();
            }
            
        }

    }
}
