namespace EventSourcing.EventStoreDB.Common;

public readonly record struct VersionedWriteResult(bool Success, long UpdatedVersion);
