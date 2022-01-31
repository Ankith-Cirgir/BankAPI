using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI
{
    public class SqlQueries
    {
        public static string CheckTabelsExist = "SELECT count(*) FROM information_schema.tables WHERE table_schema = 'bankapp' AND table_name = 'currency' LIMIT 1;";

        public static string GetAllCustomers = "Select * FROM `bankapp`.`customeraccounts`";


        public static string CreateDatabase = "CREATE DATABASE bankapp;";
        public static string CreateBanksTable = "CREATE TABLE `bankapp`.`banks` (`BankId` VARCHAR(45) NOT NULL,`BankName` VARCHAR(45) NULL,`Profits` FLOAT NULL DEFAULT 0,`sRTGSCharge` FLOAT NULL DEFAULT 0,`sIMPSCharge` FLOAT NULL DEFAULT 5,`oRTGSCharge` FLOAT NULL DEFAULT 2,`oIMPSCharge` FLOAT NULL DEFAULT 6,PRIMARY KEY(`BankId`));";
        public static string CreateCustomerAccountsTable = "CREATE TABLE `bankapp`.`customeraccounts` ( `AccountId` VARCHAR(45) NOT NULL, `BankId` VARCHAR(45) NOT NULL, `Balance` FLOAT NOT NULL, `Name` VARCHAR(45) NOT NULL, `Password` VARCHAR(45) NOT NULL, `IsActive` TINYINT NOT NULL DEFAULT 1, PRIMARY KEY (`AccountId`), INDEX `Customer BankId_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `Customer BankId` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";
        public static string CreateStaffAccountsTable = "CREATE TABLE `bankapp`.`staffaccounts` ( `AccountId` VARCHAR(45) NOT NULL, `BankId` VARCHAR(45) NOT NULL, `Name` VARCHAR(45) NOT NULL, `Password` VARCHAR(45) NOT NULL, PRIMARY KEY (`AccountId`), INDEX `staff and bankId link_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `staff and bankId link` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";
        public static string CreateTransactionsTable = "CREATE TABLE `bankapp`.`transactions` ( `TransactionId` VARCHAR(45) NOT NULL, `Amount` FLOAT NULL, `Type` INT NULL, `Time` VARCHAR(45) NULL, `SenderId` VARCHAR(45) NULL, `ReceiverId` VARCHAR(45) NULL, PRIMARY KEY (`TransactionId`), INDEX `SenderId_idx` (`SenderId` ASC) VISIBLE, INDEX `ReceiverId_idx` (`ReceiverId` ASC) VISIBLE, CONSTRAINT `SenderId` FOREIGN KEY (`SenderId`) REFERENCES `bankapp`.`customeraccounts` (`AccountId`) ON DELETE NO ACTION ON UPDATE NO ACTION, CONSTRAINT `ReceiverId` FOREIGN KEY (`ReceiverId`) REFERENCES `bankapp`.`customeraccounts` (`AccountId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";
        public static string CreateCurrencyTable = "CREATE TABLE `bankapp`.`currency` ( `currency` VARCHAR(4) NOT NULL, `BankId` VARCHAR(45) NULL, PRIMARY KEY (`currency`), INDEX `bank and currency link_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `bank and currency link` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";


        public static string GetBankProfits = "SELECT Profits from `bankapp`.`banks` where BankId = @BankId;";
        public static string GetBalance = "SELECT `Balance` from `bankapp`.`customeraccounts` where AccountId = @AccountId;";
        public static string GetName = "SELECT `Name` from `bankapp`.`customeraccounts` where AccountId = @AccountId;";
        public static string GetBankId = "SELECT `BankId` from `bankapp`.`customeraccounts` where AccountId = @AccountId;";
        public static string GetCurrencyValue = "SELECT `Value` from `bankapp`.`currency` where currency = @CurrencyName";
        public static string GetTransactions = "SELECT * from `bankapp`.`transactions` WHERE ReceiverId = @ReceiverId or SenderId = @SenderId;";
        public static string GetBanksRTGSCharges = "SELECT sRTGSCharge from `bankapp`.`banks` where BankId = @BankId;";
        public static string GetBanksIMPSCharges = "SELECT sIMPSCharge from `bankapp`.`banks` where BankId = @BankId;";
        public static string GetBankoRTGSCharges = "SELECT oRTGSCharge from `bankapp`.`banks` where BankId = @BankId;";
        public static string GetBankoIMPSCharges = "SELECT oIMPSCharge from `bankapp`.`banks` where BankId = @BankId;";
        public static string GetTransactionAmount = "SELECT Amount from `bankapp`.`transactions` WHERE TransactionId = @TransactionId";
        public static string GetTransactionType = "SELECT Type from `bankapp`.`transactions` WHERE TransactionId = @TransactionId";
        public static string GetTransactionSenderId = "SELECT SenderId from `bankapp`.`transactions` WHERE transactionId = @TransactionId;";
        public static string GetTransactionReceiverId = "SELECT ReceiverId from `bankapp`.`transactions` WHERE transactionId = @TransactionId;";
        public static string GetCustomerAccount = "SELECT AccountId, BankId, Balance, Name, Password, IsActive FROM `bankapp`.`customeraccounts` WHERE AccountId = @AccountId";
        public static string GetStaffAccount = "SELECT AccountId, BankId, Name, Password FROM `bankapp`.`staffaccounts` WHERE AccountId = @AccountId";


        public static string InsertIntoCustomersTable = "INSERT INTO `bankapp`.`customeraccounts`(`AccountId`,`BankId`,`Balance`,`Name`,`Password`)VALUES(@AccountId, @BankId, @Balance, @Name, @Password);";
        public static string InsertIntoStaffsTable = "INSERT INTO `bankapp`.`staffaccounts`(`AccountId`,`BankId`,`Name`,`Password`)VALUES(@AccountId, @BankId, @Name, @Password);";
        public static string InsertIntoBanksTable = "INSERT INTO `bankapp`.`banks`(`BankId`,`BankName`,`Profits`,`sRTGSCharge`,`sIMPSCharge`,`oRTGSCharge`,`oIMPSCharge`)VALUES(@BankId, @BankName, @Profits, @sRTGSCharge, @sIMPSCharge, @oRTGSCharge, @oIMPSCharge); ";
        public static string AddCurrency = "INSERT INTO `bankapp`.`currency` (`currency`, `BankId`, `value`) VALUES (@Currency, @BankId, @Value);";
        public static string InsertTransactionReceiver = "INSERT INTO `bankapp`.`transactions` (`TransactionId`, `Amount`, `Type`, `Time`, `ReceiverId`) VALUES (@TransactionId, @Amount, @Type, @Time, @ReceiversId);";
        public static string InsertTransaction = "INSERT INTO `bankapp`.`transactions` (`TransactionId`, `Amount`, `Type`, `Time`, `SenderId`, `ReceiverId`) VALUES (@TransactionId, @Amount, @Type, @Time, @SenderId, @ReceiversId);";


        public static string DeleteTransaction = "DELETE FROM `bankapp`.`transactions` WHERE TransactionId = @TransactionId;";
        public static string DeleteCustomerAccount = "UPDATE `bankapp`.`customeraccounts` SET  `IsActive` = 0 WHERE `AccountId` = @AccountId;";


        public static string UpdateBalance = "UPDATE `bankapp`.`customeraccounts` SET  `Balance` = @newBalance WHERE `AccountId` = @AccountId;";
        public static string UpdatePassword = "UPDATE `bankapp`.`customeraccounts` SET `Password` = '@newPassword' WHERE `AccountId` = @AccountId;";
        public static string UpdateName = "UPDATE `bankapp`.`customeraccounts` SET `Name` = @newName WHERE `AccountId` = @AccountId;";
        public static string UpdatesRTGS = "UPDATE `bankapp`.`banks` SET `sRTGSCharge` = @sRTGSCharge WHERE `BankId` = @BankId;";
        public static string UpdatesIMPS = "UPDATE `bankapp`.`banks` SET `sIMPSCharge` = @sIMPSCharge WHERE `BankId` = @BankId;";
        public static string UpdateoRTGS = "UPDATE `bankapp`.`banks` SET `oRTGSCharge` = @oRTGSCharge WHERE `BankId` = @BankId;";
        public static string UpdateoIMPS = "UPDATE `bankapp`.`banks` SET `oIMPSCharge` = @oIMPSCharge WHERE `BankId` = @BankId;";
        public static string UpdateBankProfits = "UPDATE `bankapp`.`banks` SET `Profits` = @newProfits WHERE `BankId` = @BankId;";


        public static string AuthenticateCustomer = "SELECT AccountId FROM `bankapp`.`customeraccounts` WHERE AccountId = @AccountId AND Password = @Password;";
        public static string AuthenticateStaff = "SELECT AccountId FROM `bankapp`.`staffaccounts` WHERE AccountId = @AccountId AND Password = @Password;";
    }
}
