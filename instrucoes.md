┌─────────────────────────────────────────────────────────────┐
│ API REQUEST (User tries to reserve seat)                    │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│ ReservationService.CreateReservationAsync                    │
│  ├─ AcquireLock (seat:123) ← PRIMEIRO                       │
│  ├─ Check if seat available                                 │
│  ├─ Update seat.Status = Reserved                           │
│  └─ ReleaseLock ← ÚLTIMO                                    │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│ IDistributedLockService (Redis Backend)                     │
│  ├─ AcquireLockAsync    (SET NX EX - Atômico)              │
│  ├─ ReleaseLockAsync    (Lua Script - Seguro)              │
│  └─ IsLockOwnedAsync    (GET - Validação)                  │
└─────────────────────────────────────────────────────────────┘


