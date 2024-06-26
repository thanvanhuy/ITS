using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace VVA.ITS.WebApp.Services
{
	public class PaginatedList<T> : List<T>
	{
        public int pageIndex { get; set; }
        public int totalPages { get; set; }        

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            this.pageIndex = pageIndex;
            this.totalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool hasPreviousPage => this.pageIndex > 1;
        public bool hasNextPage => this.pageIndex < this.totalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
