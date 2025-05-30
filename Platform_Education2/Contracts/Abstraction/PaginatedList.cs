using Microsoft.EntityFrameworkCore;

namespace PlatformEduPro.Contracts.Abstraction
{
    public class PaginatedList<T>
    {

      
        public PaginatedList(List<T> items,  int pageNumber,int count,int pagesize)
        {
            Items = items;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pagesize);
        }

        public List<T> Items { get;private set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get;private set; }
      

       
        public bool HasNextPage => PageNumber<TotalPages;
        public bool HasPreviousPage => PageNumber > 1;



        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T>source, int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            var count = await source.CountAsync(cancellationToken);
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, pageNumber, count, pageSize);
        }







    }
}
