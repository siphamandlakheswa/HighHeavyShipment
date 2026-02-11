namespace HighHeavyShipment.Domain;

public enum ShipmentStatus {
    QuotationRequested = 0,
    QuotationApproved = 1,
    BookingConfirmed = 2,
    InTransit = 3,
    Delivered = 4
}