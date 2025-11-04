using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Transactions.Dto;

namespace Lafise.API.services.Clients.Dto
{
    /// <summary>
    /// Summary of all accounts and their key information for a client.
    /// Provides a comprehensive overview of the client's banking status.
    /// </summary>
    public class ClientAccountsSummaryDto
    {
        /// <summary>
        /// Client's unique identifier.
        /// </summary>
        [Display(Name = "ID del Cliente", Description = "Identificador único del cliente.")]
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Client's full name.
        /// </summary>
        [Display(Name = "Nombre Completo", Description = "Nombre completo del cliente.")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Client's email address.
        /// </summary>
        [Display(Name = "Email", Description = "Dirección de correo electrónico del cliente.")]
        public string? Email { get; set; }

        /// <summary>
        /// List of all accounts with summary information.
        /// </summary>
        [Display(Name = "Cuentas", Description = "Lista de todas las cuentas del cliente con información resumida.")]
        public List<AccountSummaryDto> Accounts { get; set; } = new List<AccountSummaryDto>();

        /// <summary>
        /// Total number of accounts owned by the client.
        /// </summary>
        [Display(Name = "Total de Cuentas", Description = "Número total de cuentas del cliente.")]
        [DefaultValue(2)]
        public int TotalAccounts { get; set; }

        /// <summary>
        /// Combined balance across all accounts.
        /// </summary>
        [Display(Name = "Saldo Total", Description = "Saldo combinado de todas las cuentas.")]
        [DefaultValue(15000.00)]
        public decimal TotalBalance { get; set; }
    }

    /// <summary>
    /// Summary information for a single account including transaction statistics.
    /// </summary>
    public class AccountSummaryDto
    {
        /// <summary>
        /// Account number.
        /// </summary>
        [Display(Name = "Número de Cuenta", Description = "Número único de la cuenta.")]
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// Type of account (e.g., Savings, Checking).
        /// </summary>
        [Display(Name = "Tipo de Cuenta", Description = "Tipo de cuenta (Ahorros, Corriente, etc.).")]
        public string AccountType { get; set; } = string.Empty;

        /// <summary>
        /// Current balance of the account.
        /// </summary>
        [Display(Name = "Saldo Actual", Description = "Saldo actual de la cuenta.")]
        [DefaultValue(5000.00)]
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Date when the account was created.
        /// </summary>
        [Display(Name = "Fecha de Creación", Description = "Fecha en que se creó la cuenta.")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Transaction summary for this account.
        /// </summary>
        [Display(Name = "Resumen de Transacciones", Description = "Resumen estadístico de las transacciones de la cuenta.")]
        public TransactionSummaryDto TransactionSummary { get; set; } = new TransactionSummaryDto();

        /// <summary>
        /// Total number of transactions for this account.
        /// </summary>
        [Display(Name = "Total de Transacciones", Description = "Número total de transacciones en la cuenta.")]
        [DefaultValue(25)]
        public int TotalTransactions { get; set; }

        /// <summary>
        /// Date of the most recent transaction.
        /// </summary>
        [Display(Name = "Última Transacción", Description = "Fecha de la transacción más reciente.")]
        public DateTime? LastTransactionDate { get; set; }
    }
}

