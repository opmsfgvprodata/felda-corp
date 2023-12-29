﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_SYSTEM.ModelsSP
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using System.Collections.Generic;

    public partial class MVC_SYSTEM_SP2_Models : DbContext
    {
        public MVC_SYSTEM_SP2_Models()
            : base("name=MVC_SYSTEM_SP2_CONN")
        {
            SetCommandTimeout(this.Database.Connection.ConnectionTimeout);
        }

        public void SetCommandTimeout(int timeout)
        {
            var objcontxt = (this as IObjectContextAdapter).ObjectContext;
            objcontxt.CommandTimeout = this.Database.Connection.ConnectionTimeout;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual IEnumerable<sp_DashAllKerakyatan_Result> sp_DashAllKerakyatan(Nullable<int> syarikatID, Nullable<int> type)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var typeParameter = type.HasValue ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DashAllKerakyatan_Result>("sp_DashAllKerakyatan", syarikatIDParameter, typeParameter);
        }
    
        public virtual IEnumerable<sp_DashJantina_Result> sp_DashJantina(Nullable<int> syarikatID, Nullable<int> wilayah, Nullable<int> costcentre)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahParameter = wilayah.HasValue ?
                new ObjectParameter("Wilayah", wilayah) :
                new ObjectParameter("Wilayah", typeof(int));
    
            var costcentreParameter = costcentre.HasValue ?
                new ObjectParameter("Costcentre", costcentre) :
                new ObjectParameter("Costcentre", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DashJantina_Result>("sp_DashJantina", syarikatIDParameter, wilayahParameter, costcentreParameter);
        }
    
        public virtual IEnumerable<sp_DashKerakyatan_Result> sp_DashKerakyatan(Nullable<int> syarikatID, Nullable<int> wilayah, Nullable<int> costcentre)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahParameter = wilayah.HasValue ?
                new ObjectParameter("Wilayah", wilayah) :
                new ObjectParameter("Wilayah", typeof(int));
    
            var costcentreParameter = costcentre.HasValue ?
                new ObjectParameter("Costcentre", costcentre) :
                new ObjectParameter("Costcentre", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DashKerakyatan_Result>("sp_DashKerakyatan", syarikatIDParameter, wilayahParameter, costcentreParameter);
        }
    
        public virtual IEnumerable<sp_DashLadang_Result> sp_DashLadang(Nullable<int> syarikatID, Nullable<int> wilayah, Nullable<int> costcentre)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahParameter = wilayah.HasValue ?
                new ObjectParameter("Wilayah", wilayah) :
                new ObjectParameter("Wilayah", typeof(int));
    
            var costcentreParameter = costcentre.HasValue ?
                new ObjectParameter("Costcentre", costcentre) :
                new ObjectParameter("Costcentre", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DashLadang_Result>("sp_DashLadang", syarikatIDParameter, wilayahParameter, costcentreParameter);
        }
    
        public virtual IEnumerable<sp_DashPermitExpired_Result> sp_DashPermitExpired(Nullable<int> syarikatID, Nullable<int> type)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var typeParameter = type.HasValue ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DashPermitExpired_Result>("sp_DashPermitExpired", syarikatIDParameter, typeParameter);
        }
    
        public virtual IEnumerable<sp_DashStatusAkaun_Result> sp_DashStatusAkaun(Nullable<int> syarikatID, Nullable<int> year, Nullable<int> month, Nullable<int> type, Nullable<int> wilayah, string costCenter)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var typeParameter = type.HasValue ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(int));
    
            var wilayahParameter = wilayah.HasValue ?
                new ObjectParameter("Wilayah", wilayah) :
                new ObjectParameter("Wilayah", typeof(int));
    
            var costCenterParameter = costCenter != null ?
                new ObjectParameter("CostCenter", costCenter) :
                new ObjectParameter("CostCenter", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DashStatusAkaun_Result>("sp_DashStatusAkaun", syarikatIDParameter, yearParameter, monthParameter, typeParameter, wilayahParameter, costCenterParameter);
        }
    
        public virtual IEnumerable<sp_DashTransactionListing_Result> sp_DashTransactionListing(Nullable<int> syarikatID, Nullable<int> year)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DashTransactionListing_Result>("sp_DashTransactionListing", syarikatIDParameter, yearParameter);
        }
    
        public virtual IEnumerable<sp_DashWilayah_Result> sp_DashWilayah(Nullable<int> syarikatID, Nullable<int> type)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var typeParameter = type.HasValue ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DashWilayah_Result>("sp_DashWilayah", syarikatIDParameter, typeParameter);
        }
    
        public virtual IEnumerable<sp_DatatableJantina_Result> sp_DatatableJantina(Nullable<int> syarikatID, Nullable<int> wilayah, string jantina, Nullable<int> costcentre)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahParameter = wilayah.HasValue ?
                new ObjectParameter("Wilayah", wilayah) :
                new ObjectParameter("Wilayah", typeof(int));
    
            var jantinaParameter = jantina != null ?
                new ObjectParameter("Jantina", jantina) :
                new ObjectParameter("Jantina", typeof(string));
    
            var costcentreParameter = costcentre.HasValue ?
                new ObjectParameter("Costcentre", costcentre) :
                new ObjectParameter("Costcentre", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DatatableJantina_Result>("sp_DatatableJantina", syarikatIDParameter, wilayahParameter, jantinaParameter, costcentreParameter);
        }
    
        public virtual IEnumerable<sp_DatatableKerakyatan_Result> sp_DatatableKerakyatan(Nullable<int> syarikatID, Nullable<int> wilayah, string kerakyatan, Nullable<int> costcentre)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahParameter = wilayah.HasValue ?
                new ObjectParameter("Wilayah", wilayah) :
                new ObjectParameter("Wilayah", typeof(int));
    
            var kerakyatanParameter = kerakyatan != null ?
                new ObjectParameter("Kerakyatan", kerakyatan) :
                new ObjectParameter("Kerakyatan", typeof(string));
    
            var costcentreParameter = costcentre.HasValue ?
                new ObjectParameter("Costcentre", costcentre) :
                new ObjectParameter("Costcentre", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DatatableKerakyatan_Result>("sp_DatatableKerakyatan", syarikatIDParameter, wilayahParameter, kerakyatanParameter, costcentreParameter);
        }
    
        public virtual IEnumerable<sp_DatatableLadang_Result> sp_DatatableLadang(Nullable<int> syarikatID, Nullable<int> wilayah, string ladang, Nullable<int> costcentre)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahParameter = wilayah.HasValue ?
                new ObjectParameter("Wilayah", wilayah) :
                new ObjectParameter("Wilayah", typeof(int));
    
            var ladangParameter = ladang != null ?
                new ObjectParameter("Ladang", ladang) :
                new ObjectParameter("Ladang", typeof(string));
    
            var costcentreParameter = costcentre.HasValue ?
                new ObjectParameter("Costcentre", costcentre) :
                new ObjectParameter("Costcentre", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DatatableLadang_Result>("sp_DatatableLadang", syarikatIDParameter, wilayahParameter, ladangParameter, costcentreParameter);
        }
    
        public virtual IEnumerable<sp_DatatablePermitExpired_Result> sp_DatatablePermitExpired(Nullable<int> syarikatID, Nullable<int> wilayah, Nullable<int> type, Nullable<int> costcentre)
        {
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahParameter = wilayah.HasValue ?
                new ObjectParameter("Wilayah", wilayah) :
                new ObjectParameter("Wilayah", typeof(int));
    
            var typeParameter = type.HasValue ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(int));
    
            var costcentreParameter = costcentre.HasValue ?
                new ObjectParameter("Costcentre", costcentre) :
                new ObjectParameter("Costcentre", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_DatatablePermitExpired_Result>("sp_DatatablePermitExpired", syarikatIDParameter, wilayahParameter, typeParameter, costcentreParameter);
        }
    
        public virtual IEnumerable<sp_MaybankRcms_Result> sp_MaybankRcms(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> year, Nullable<int> month, Nullable<int> userID, string compCode)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var compCodeParameter = compCode != null ?
                new ObjectParameter("CompCode", compCode) :
                new ObjectParameter("CompCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_MaybankRcms_Result>("sp_MaybankRcms", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, yearParameter, monthParameter, userIDParameter, compCodeParameter);
        }
    
        public virtual IEnumerable<sp_MyegDetail_Result> sp_MyegDetail(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_MyegDetail_Result>("sp_MyegDetail", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter);
        }
    
        public virtual IEnumerable<sp_PaymentModeReport_Result> sp_PaymentModeReport(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> year, Nullable<int> month, Nullable<int> userID, string compCode)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var compCodeParameter = compCode != null ?
                new ObjectParameter("CompCode", compCode) :
                new ObjectParameter("CompCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_PaymentModeReport_Result>("sp_PaymentModeReport", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, yearParameter, monthParameter, userIDParameter, compCodeParameter);
        }
    
        public virtual IEnumerable<sp_PermitPassportDetail_Result> sp_PermitPassportDetail(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, string kodAktif)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var kodAktifParameter = kodAktif != null ?
                new ObjectParameter("KodAktif", kodAktif) :
                new ObjectParameter("KodAktif", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_PermitPassportDetail_Result>("sp_PermitPassportDetail", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, kodAktifParameter);
        }
    
        public virtual IEnumerable<sp_RptBulPenPekLad_Result> sp_RptBulPenPekLad(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, string kdrytan, Nullable<int> month, Nullable<int> year, Nullable<int> userID, string costCentre)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var kdrytanParameter = kdrytan != null ?
                new ObjectParameter("Kdrytan", kdrytan) :
                new ObjectParameter("Kdrytan", typeof(string));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var costCentreParameter = costCentre != null ?
                new ObjectParameter("CostCentre", costCentre) :
                new ObjectParameter("CostCentre", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptBulPenPekLad_Result>("sp_RptBulPenPekLad", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, kdrytanParameter, monthParameter, yearParameter, userIDParameter, costCentreParameter);
        }
    
        public virtual IEnumerable<sp_RptGajiMinima_Result> sp_RptGajiMinima(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> month, Nullable<int> year, Nullable<int> userID, string costCentre)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var costCentreParameter = costCentre != null ?
                new ObjectParameter("CostCentre", costCentre) :
                new ObjectParameter("CostCentre", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptGajiMinima_Result>("sp_RptGajiMinima", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, monthParameter, yearParameter, userIDParameter, costCentreParameter);
        }
    
        public virtual IEnumerable<sp_RptMakPekTem_Result> sp_RptMakPekTem(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> aktifStatus, Nullable<int> userID, string costCentre)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var aktifStatusParameter = aktifStatus.HasValue ?
                new ObjectParameter("AktifStatus", aktifStatus) :
                new ObjectParameter("AktifStatus", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var costCentreParameter = costCentre != null ?
                new ObjectParameter("CostCentre", costCentre) :
                new ObjectParameter("CostCentre", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptMakPekTem_Result>("sp_RptMakPekTem", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, aktifStatusParameter, userIDParameter, costCentreParameter);
        }
    
        public virtual IEnumerable<sp_RptMasterDataPkj_Result> sp_RptMasterDataPkj(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, string kdrytan, string statusAktif, string kategoriPkj, Nullable<int> userID, string costCentre)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var kdrytanParameter = kdrytan != null ?
                new ObjectParameter("Kdrytan", kdrytan) :
                new ObjectParameter("Kdrytan", typeof(string));
    
            var statusAktifParameter = statusAktif != null ?
                new ObjectParameter("StatusAktif", statusAktif) :
                new ObjectParameter("StatusAktif", typeof(string));
    
            var kategoriPkjParameter = kategoriPkj != null ?
                new ObjectParameter("KategoriPkj", kategoriPkj) :
                new ObjectParameter("KategoriPkj", typeof(string));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var costCentreParameter = costCentre != null ?
                new ObjectParameter("CostCentre", costCentre) :
                new ObjectParameter("CostCentre", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptMasterDataPkj_Result>("sp_RptMasterDataPkj", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, kdrytanParameter, statusAktifParameter, kategoriPkjParameter, userIDParameter, costCentreParameter);
        }
    
        public virtual IEnumerable<sp_RptRumKedKepPekLad_Result> sp_RptRumKedKepPekLad(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> aktifStatus, Nullable<int> userID, string compCode)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var aktifStatusParameter = aktifStatus.HasValue ?
                new ObjectParameter("AktifStatus", aktifStatus) :
                new ObjectParameter("AktifStatus", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var compCodeParameter = compCode != null ?
                new ObjectParameter("CompCode", compCode) :
                new ObjectParameter("CompCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptRumKedKepPekLad_Result>("sp_RptRumKedKepPekLad", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, aktifStatusParameter, userIDParameter, compCodeParameter);
        }
    
        public virtual int sp_RptSkb(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> month, Nullable<int> year)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_RptSkb", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, monthParameter, yearParameter);
        }
    
        public virtual IEnumerable<sp_MapaReport_Result> sp_MapaReport(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> year, Nullable<int> month, Nullable<int> userID, string compCode)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var compCodeParameter = compCode != null ?
                new ObjectParameter("CompCode", compCode) :
                new ObjectParameter("CompCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_MapaReport_Result>("sp_MapaReport", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, yearParameter, monthParameter, userIDParameter, compCodeParameter);
        }
    
        public virtual IEnumerable<sp_RptPermohonanPekerjaBru_Result> sp_RptPermohonanPekerjaBru(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> year, Nullable<int> month, Nullable<int> statusApproved, string compCode)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var statusApprovedParameter = statusApproved.HasValue ?
                new ObjectParameter("StatusApproved", statusApproved) :
                new ObjectParameter("StatusApproved", typeof(int));
    
            var compCodeParameter = compCode != null ?
                new ObjectParameter("CompCode", compCode) :
                new ObjectParameter("CompCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptPermohonanPekerjaBru_Result>("sp_RptPermohonanPekerjaBru", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, yearParameter, monthParameter, statusApprovedParameter, compCodeParameter);
        }
    
        public virtual IEnumerable<sp_MaybankTNG_Result> sp_MaybankTNG(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> year, Nullable<int> month, Nullable<int> userID, string compCode)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var compCodeParameter = compCode != null ?
                new ObjectParameter("CompCode", compCode) :
                new ObjectParameter("CompCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_MaybankTNG_Result>("sp_MaybankTNG", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, yearParameter, monthParameter, userIDParameter, compCodeParameter);
        }
    }
}
