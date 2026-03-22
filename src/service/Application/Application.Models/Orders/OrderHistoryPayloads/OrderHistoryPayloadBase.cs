using System.Text.Json.Serialization;

namespace Application.Models.Orders.OrderHistoryPayloads;

[JsonDerivedType(typeof(HistoryCreatedPayload), typeDiscriminator: nameof(HistoryCreatedPayload))]
[JsonDerivedType(typeof(HistoryItemAddedPayload), typeDiscriminator: nameof(HistoryItemAddedPayload))]
[JsonDerivedType(typeof(HistoryItemRemovedPayload), typeDiscriminator: nameof(HistoryItemRemovedPayload))]
[JsonDerivedType(typeof(HistoryStateChangedPayload), typeDiscriminator: nameof(HistoryStateChangedPayload))]
[JsonDerivedType(typeof(HistoryProcessingStateChangedPayload), typeDiscriminator: nameof(HistoryProcessingStateChangedPayload))]
public record OrderHistoryPayloadBase;