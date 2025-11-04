using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.controllers.Dto
{
    /// <summary>
    /// Generic data transfer object for paginated results.
    /// Provides a standardized structure for API responses that include paginated data.
    /// </summary>
    /// <typeparam name="T">The type of data elements contained in the paginated response.</typeparam>
    public class PagedDto<T>
    {
        /// <summary>
        /// Initializes a new instance of the PagedDto class with an empty data collection.
        /// </summary>
        public PagedDto()
        {
            Data = new List<T>();
            Pagination = new Pagination();
        }

        /// <summary>
        /// Gets or sets the collection of data items for the current page.
        /// </summary>
        /// <value>A list containing the data elements for the current page.</value>
        [Display(Name = "Datos", Description = "Lista de elementos correspondientes a la página actual.")]
        public List<T> Data { get; set; }

        /// <summary>
        /// Gets or sets the pagination metadata associated with this response.
        /// Contains information about the current page, total pages, and other pagination details.
        /// </summary>
        /// <value>A Pagination object containing metadata about the pagination state.</value>
        [Display(Name = "Paginación", Description = "Información de paginación asociada a la respuesta.")]
        public Pagination Pagination { get; set; }
    }

    /// <summary>
    /// Contains pagination metadata for paginated API responses.
    /// Provides information about the current page, total number of items, and other pagination details.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Gets or sets the number of items in the current page.
        /// </summary>
        /// <value>An integer representing the item count in the current page.</value>
        [Display(Name = "Cantidad de elementos", Description = "Número de elementos en la página actual.")]
        [DefaultValue(10)]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the total number of items across all pages.
        /// </summary>
        /// <value>An integer representing the total number of items available.</value>
        [Display(Name = "Total de elementos", Description = "Número total de elementos disponibles.")]
        [DefaultValue(100)]
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// Page numbering starts at 1 for the first page.
        /// </summary>
        /// <value>An integer representing the current page number (1-based).</value>
        [Display(Name = "Página actual", Description = "Número de la página actual (comenzando desde 1).")]
        [DefaultValue(1)]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available.
        /// </summary>
        /// <value>An integer representing the total number of pages.</value>
        [Display(Name = "Total de páginas", Description = "Número total de páginas disponibles.")]
        [DefaultValue(10)]
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the number of items displayed per page.
        /// </summary>
        /// <value>An integer representing the page size (items per page).</value>
        [Display(Name = "Tamaño de página", Description = "Número de elementos por página.")]
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }

    /// <summary>
    /// Represents a paginated response that includes both data and summary information.
    /// Extends the base PagedDto to include additional summary data of type TSummary.
    /// </summary>
    /// <typeparam name="TData">The type of data contained in the paginated response.</typeparam>
    /// <typeparam name="TSummary">The type of summary information included with the paginated data.</typeparam>
    public class PagedDtoSummary<TData, TSummary> : PagedDto<TData>
    {
        /// <summary>
        /// Gets or sets the summary information associated with the paginated data.
        /// </summary>
        /// <value>The summary data of type TSummary.</value>
        [Display(Description = "Summary information associated with the paginated data")]
        public TSummary Summary { get; set; }
    }
}

