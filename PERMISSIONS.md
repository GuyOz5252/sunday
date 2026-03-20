# FOS — Permissions Matrix

---

## Role Hierarchy

```
SuperAdmin
  └── AgencyAdmin
        ├── TrafficManager
        ├── CreativeManager
        ├── AccountManager
        ├── Designer
        ├── Copywriter
        └── External (Phase 3)
```

---

## RBAC Matrix

### Tasks

| Action | SuperAdmin | AgencyAdmin | TM | CM | AM | Designer | CW | External |
|--------|-----------|------------|----|----|----|---------|----|---------|
| Create brief | ✓ | ✓ | ✓ | — | ✓ | — | — | — |
| View all tasks in agency | ✓ | ✓ | ✓ | ✓ | — | — | — | — |
| View own client tasks | ✓ | ✓ | ✓ | ✓ | ✓ | — | — | — |
| View assigned tasks | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Edit brief | ✓ | ✓ | — | — | ✓(own) | — | — | — |
| Assign task | ✓ | ✓ | ✓ | — | — | — | — | — |
| Change priority | ✓ | ✓ | ✓ | — | — | — | — | — |
| Change deadline | ✓ | ✓ | ✓ | — | — | — | — | — |
| Bulk assign | ✓ | ✓ | ✓ | — | — | — | — | — |
| Approve (creative) | ✓ | ✓ | — | ✓ | — | — | — | — |
| Approve (AM) | ✓ | ✓ | — | — | ✓(own) | — | — | — |
| Return to revision | ✓ | ✓ | — | ✓ | ✓ | — | — | — |
| Mark ready for review | ✓ | ✓ | — | — | — | ✓ | ✓ | ✓ |
| Cancel task | ✓ | ✓ | ✓ | — | ✓(draft/new) | — | — | — |
| Reopen closed task | ✓ | ✓ | ✓ | — | — | — | — | — |
| Delete task | ✓ | ✓ | — | — | — | — | — | — |
| Clone task (Phase 2) | ✓ | ✓ | ✓ | — | ✓ | — | — | — |

### Traffic Board

| Action | SuperAdmin | AgencyAdmin | TM | CM | AM | Designer | CW | External |
|--------|-----------|------------|----|----|----|---------|----|---------|
| View traffic board | ✓ | ✓ | ✓ | ✓(read) | — | — | — | — |
| Drag & drop reassign | ✓ | ✓ | ✓ | — | — | — | — | — |
| View workload chart | ✓ | ✓ | ✓ | ✓ | — | — | — | — |

### Time Tracking

| Action | SuperAdmin | AgencyAdmin | TM | CM | AM | Designer | CW | External |
|--------|-----------|------------|----|----|----|---------|----|---------|
| Start/stop own timer | — | — | — | — | — | ✓ | ✓ | ✓ |
| View own time entries | — | — | — | — | — | ✓ | ✓ | ✓ |
| View all time entries | — | ✓ | ✓ | ✓ | — | — | — | — |
| View total time on own tasks | ✓ | ✓ | ✓ | ✓ | ✓(total only) | ✓(own) | ✓(own) | ✓(own) |
| View time + cost calculations | — | ✓ | — | — | — | — | — | — |
| Edit time entries | — | ✓ | — | — | — | — | — | — |

### Files

| Action | SuperAdmin | AgencyAdmin | TM | CM | AM | Designer | CW | External |
|--------|-----------|------------|----|----|----|---------|----|---------|
| Upload file | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Delete any file | ✓ | ✓ | — | — | — | — | — | — |
| Delete own file | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Mark file as final | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | — |
| Mark file as client-facing | ✓ | ✓ | — | ✓ | ✓ | — | — | — |

### Chat

| Action | SuperAdmin | AgencyAdmin | TM | CM | AM | Designer | CW | External |
|--------|-----------|------------|----|----|----|---------|----|---------|
| Send message on accessible task | ✓ | ✓ | ✓ | ✓ | ✓(own) | ✓(own) | ✓(own) | ✓(own) |
| Delete own message | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Delete any message | ✓ | ✓ | — | — | — | — | — | — |

### Analytics

| Action | SuperAdmin | AgencyAdmin | TM | CM | AM | Designer | CW | External |
|--------|-----------|------------|----|----|----|---------|----|---------|
| View agency analytics | — | ✓ | ✓ | ✓ | — | — | — | — |
| View client analytics | — | ✓ | ✓ | ✓ | ✓(own) | — | — | — |
| View employee analytics | — | ✓ | ✓ | — | — | — | — | — |
| View profitability (Phase 3) | — | ✓ | — | — | — | — | — | — |
| Export analytics | — | ✓ | ✓ | — | — | — | — | — |
| View My Stats | — | — | — | — | — | ✓ | ✓ | ✓ |

### Users & Admin

| Action | SuperAdmin | AgencyAdmin | TM | CM | AM | Designer | CW | External |
|--------|-----------|------------|----|----|----|---------|----|---------|
| Invite users | ✓ | ✓ | — | — | — | — | — | — |
| Bulk invite users (Phase 2) | ✓ | ✓ | — | — | — | — | — | — |
| Edit user roles | ✓ | ✓ | — | — | — | — | — | — |
| Set user availability | ✓ | ✓ | ✓ | — | — | — | — | — |
| Deactivate user | ✓ | ✓ | — | — | — | — | — | — |
| Manage clients | ✓ | ✓ | — | — | — | — | — | — |
| Assign AM to client | ✓ | ✓ | — | — | — | — | — | — |
| Manage workflow rules | ✓ | ✓ | — | — | — | — | — | — |
| Manage SLA policies | ✓ | ✓ | — | — | — | — | — | — |
| Manage hourly rates | ✓ | ✓ | — | — | — | — | — | — |
| Create approval delegation | ✓ | ✓ | — | ✓(self) | ✓(self) | — | — | — |
| View audit log | ✓ | ✓ | — | — | — | — | — | — |

### Agencies (SuperAdmin only)

| Action | SuperAdmin |
|--------|-----------|
| Create agency | ✓ |
| Activate / deactivate agency | ✓ |
| Impersonate agency | ✓ |
| View platform stats | ✓ |

---

## Contextual Access (ABAC)

Beyond role, a user has access to a task if ANY of:
1. They created the task (AM)
2. They are the current or past assignee (`TaskAssignment`)
3. They are a Watcher on the task (`TaskWatcher`)
4. They are the AM linked to the client (`AccountManagerClient`)
5. They are TM, CM, or AgencyAdmin (agency-wide access)

---

## TaskWatcher Access

Watchers receive all notifications but CANNOT:
- Change status
- Approve or reject
- Assign or reassign
- Edit brief or fields

Watchers CAN:
- Read all content on the task (brief, files, chat, status history)
- Send messages in chat
- Upload files

---

## Approval Delegation (Phase 2)

When delegation is active:
- Delegatee has same approval permissions as delegator for the duration
- All delegated actions logged with: `{ by: delegateeId, onBehalfOf: delegatorId }`
- Original delegator still receives notifications but actions are attributed correctly
