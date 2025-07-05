import { environment } from "src/environment";

export const URL_LIST={
    //Login
    LOGIN:`${environment.BaseUrl}Login/Login`,
    REGISTER:`${environment.BaseUrl}Login/Register`,

    //Nav
    FEATURES:`${environment.BaseUrl}Auth/Features`,

    //Stocks
    GET_STOCK_DETAILS:`${environment.BaseUrl}Stocks/StocksHoldings`,
    ADD_STOCK_DETAILS:`${environment.BaseUrl}Stocks/UploadGrowwReport`,
    DELETE_STOCK_DETAILS:`${environment.BaseUrl}Investment/DeleteStockData`,

    //Mf
    GET_MF_DETAILS:`${environment.BaseUrl}MutualFund/MutualFundHoldings`,
    ADD_MF_DETAILS:`${environment.BaseUrl}MutualFund/UploadGrowwReport`,
    GET_MF_DAYPERFORMANCE_DETAILS:`${environment.BaseUrl}MutualFund/GetData`,
    TRIGGER_MF_DAYPERFORMANCE_DETAILS:`${environment.BaseUrl}MutualFund/DayPerformanceMF`,
    GET_SCHEME_NAMES:`${environment.BaseUrl}MutualFund/GetSchemeNames`,

    //Vault
    GET_FILES:`${environment.BaseUrl}Vault/all`,
    VIEW_FILE:`${environment.BaseUrl}files/view`,
    DOWNLOAD_FILE:`${environment.BaseUrl}Vault/download`,
    DELETE_FILE:`${environment.BaseUrl}Vault/files/delete`
}