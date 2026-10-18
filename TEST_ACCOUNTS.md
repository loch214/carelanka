# Test accounts

One demo account for trying CareLanka. Everyone is welcome to use it.

| Role | Sign in with | Password |
| :--- | :--- | :--- |
| Reception (general staff) | `staff.jayasuriya@carelanka.lk` | `CareLanka#2026` |

Sign in on the web app, or call `POST /api/auth/login` with the email and password.

The other staff, crew and patient accounts on the hosted site have private passwords and are not
listed here. A local database seeded from `docs/seed/` still uses the passwords written in those
scripts.

### What this account can do
Registers and admits walk-in patients and edits their details · assigns a bed · marks a patient
arrived · checks in a booking · raises, itemises and settles a bill · confirms a discharge · reads
the bookings list, wards and bed capacity.
**Cannot:** tick clinical clearance, set prices, or change a patient's name, gender or NIC once a
NIC is on the record (an unidentified patient with no NIC yet can still be fully edited).

---

## Using a token

Sign in, copy `access_token`, send it as `Authorization: Bearer <token>`. In Swagger, click
**Authorize** and paste the token **without** typing `Bearer` in front — Swagger adds that
itself, and pasting it twice is the usual reason a token "does not work".

An access token lasts **15 minutes**. `POST /api/auth/refresh` with your `refresh_token`
gives you a fresh pair. Each refresh token works **once**; reusing an old one deliberately
ends every session that account has.

| What you see | What it usually is |
| :--- | :--- |
| `401` on a password you are sure about | The seed script has not been run |
| `401` on everything, suddenly | The access token is over 15 minutes old. Refresh it |
| `403`, not `401` | You are signed in fine. That role is not allowed to do that |
| `429` | Too many attempts. Wait a minute |
| `500` on anything that reads data | PostgreSQL is not reachable. `/api/health` says so outright |
