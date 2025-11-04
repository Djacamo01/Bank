using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.services.Transactions.Dto
{
    /// <summary>
    /// Summary of transaction statistics for an account.
    /// Provides aggregated information about deposits, withdrawals, and transfers.
    /// </summary>
    public class TransactionSummaryDto
    {
        /// <summary>
        /// Total number of deposit transactions.
        /// </summary>
        [Display(Name = "Total de depósitos", Description = "Número total de transacciones de depósito.")]
        [DefaultValue(5)]
        public int TotalDeposits { get; set; }

        /// <summary>
        /// Total amount of all deposits.
        /// </summary>
        [Display(Name = "Monto total de depósitos", Description = "Suma total de todos los depósitos.")]
        [DefaultValue(5000.00)]
        public decimal TotalDepositsAmount { get; set; }

        /// <summary>
        /// Total number of withdrawal transactions.
        /// </summary>
        [Display(Name = "Total de retiros", Description = "Número total de transacciones de retiro.")]
        [DefaultValue(3)]
        public int TotalWithdrawals { get; set; }

        /// <summary>
        /// Total amount of all withdrawals.
        /// </summary>
        [Display(Name = "Monto total de retiros", Description = "Suma total de todos los retiros.")]
        [DefaultValue(1500.00)]
        public decimal TotalWithdrawalsAmount { get; set; }

        /// <summary>
        /// Total number of transfer out transactions (money sent to other accounts).
        /// </summary>
        [Display(Name = "Total de transferencias salientes", Description = "Número total de transferencias enviadas a otras cuentas.")]
        [DefaultValue(2)]
        public int TotalTransfersOut { get; set; }

        /// <summary>
        /// Total amount of all transfer out transactions.
        /// </summary>
        [Display(Name = "Monto total de transferencias salientes", Description = "Suma total de todas las transferencias enviadas.")]
        [DefaultValue(1000.00)]
        public decimal TotalTransfersOutAmount { get; set; }

        /// <summary>
        /// Total number of transfer in transactions (money received from other accounts).
        /// </summary>
        [Display(Name = "Total de transferencias entrantes", Description = "Número total de transferencias recibidas de otras cuentas.")]
        [DefaultValue(1)]
        public int TotalTransfersIn { get; set; }

        /// <summary>
        /// Total amount of all transfer in transactions.
        /// </summary>
        [Display(Name = "Monto total de transferencias entrantes", Description = "Suma total de todas las transferencias recibidas.")]
        [DefaultValue(500.00)]
        public decimal TotalTransfersInAmount { get; set; }

        /// <summary>
        /// Net amount (total deposits + transfers in - total withdrawals - transfers out).
        /// </summary>
        [Display(Name = "Monto neto", Description = "Diferencia entre ingresos (depósitos + transferencias entrantes) y egresos (retiros + transferencias salientes).")]
        [DefaultValue(3000.00)]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Current account balance at the time of the query.
        /// </summary>
        [Display(Name = "Saldo actual", Description = "Saldo actual de la cuenta al momento de la consulta.")]
        [DefaultValue(3500.00)]
        public decimal CurrentBalance { get; set; }
    }
}

