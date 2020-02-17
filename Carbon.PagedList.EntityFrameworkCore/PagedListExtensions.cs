﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.PagedList.EntityFrameworkCore
{
    public static class PagedListExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> superset, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber cannot be below 1.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "PageSize cannot be less than 1.");

            var TotalItemCount = superset == null ? 0 : superset.Count();
            var result = new List<T>();
            if (superset != null && TotalItemCount > 0)
            {
                if (pageNumber == 1)
                    result = await superset.Skip(0).Take(pageSize).ToListAsync();
                else
                    result = await superset.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            }

            return new PagedList<T>(result, pageNumber, pageSize);
        }
    }
}