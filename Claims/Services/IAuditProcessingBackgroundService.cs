namespace Claims.Services
{
    public interface IAuditProcessingBackgroundService
    {
        void EnqueueAudit(Task auditTask);

    }
}
