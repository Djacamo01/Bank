using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.controllers.Dto
{
    /// <summary>
    /// Data transfer object for pagination request parameters.
    /// Used to specify page number and page size for paginated API endpoints.
    /// </summary>
    public class PaginationRequestDto
    {
        /// <summary>
        /// Gets or sets the page number to retrieve.
        /// Page numbering starts at 1 for the first page.
        /// </summary>
        /// <value>An integer representing the page number (1-based). Default is 1.</value>
        [Display(Name = "Página", Description = "Número de página a recuperar (comenzando desde 1).")]
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0.")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items to retrieve per page.
        /// </summary>
        /// <value>An integer representing the page size (items per page). Default is 10.</value>
        [Display(Name = "Tamaño de página", Description = "Número de elementos por página.")]
        [DefaultValue(10)]
        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
        public int PageSize { get; set; } = 10;
    }
}

