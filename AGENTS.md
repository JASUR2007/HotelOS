## Goal
Fix all outstanding UI/UX issues across the hotel booking system.

## Progress
### Done
- **Audit logs empty** — `GatewayAuditLog` nullable strings (was crashing on NULL `UserName`).
- **Missing permissions** — Added `view_housekeeping` across all layers.
- **Receptionist login → access-denied** — `RoleSeeder` rewritten to sync (remove stale + add missing).
- **Housekeeping route/sidebar** — Changed from `manage_rooms` → `view_housekeeping`.
- **Payment PK violation** — `FixIdentityColumnAsync` with `has_default` guard, container rebuilt.
- **Bookings: room selection** — Added room dropdown in Check-In form; backend `CheckInRequestDto` accepts optional `RoomId`.
- **RoomDetails: past dates blocked** — Added `min` attribute to date inputs (today for check-in, check-in for check-out).
- **Containers rebuilt**: gateway-api, user-service, payment-service, frontend (×2), reception-service.

### Remaining / Future
- `change_housekeeping` permission doesn't exist — create if needed.
- Verify receptionist dashboard works after localStorage clear + re-login.

## Relevant Files
- `frontend/src/admin/pages/Bookings.tsx` — Added room dropdown, `min` on dates, `roomId` in form state.
- `frontend/src/pages/RoomDetails.tsx` — Added `min` on check-in/check-out date inputs.
- `frontend/src/api/index.ts` — `guestCheckIn` now accepts optional `roomId`.
- `backend/services/reception-service/DTOs/ReceptionDtos.cs` — `CheckInRequestDto` has optional `RoomId`.
- `backend/services/reception-service/Services/ReceptionService.cs` — `CheckInAsync` uses `RoomId` when provided.
