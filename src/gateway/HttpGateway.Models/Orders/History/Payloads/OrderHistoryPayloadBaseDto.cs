using System.Text.Json.Serialization;

namespace HttpGateway.Models.Orders.History.Payloads;

[JsonDerivedType(typeof(HistoryCreatedPayloadDto), typeDiscriminator: nameof(HistoryCreatedPayloadDto))]
[JsonDerivedType(typeof(HistoryItemAddedPayloadDto), typeDiscriminator: nameof(HistoryItemAddedPayloadDto))]
[JsonDerivedType(typeof(HistoryItemRemovedPayloadDto), typeDiscriminator: nameof(HistoryItemRemovedPayloadDto))]
[JsonDerivedType(typeof(HistoryStateChangedPayloadDto), typeDiscriminator: nameof(HistoryStateChangedPayloadDto))]
[JsonDerivedType(typeof(HistoryProcessingPayloadDto), typeDiscriminator: nameof(HistoryProcessingPayloadDto))]
public record OrderHistoryPayloadBaseDto;