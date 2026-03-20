# FOS — Studio Workflow Management System
## Product Requirements Document (PRD)

---

## 1. Product Overview

FOS is a multi-tenant Studio Workflow Management System for advertising agencies. Its core is a studio module in which every creative request follows a structured approval path from brief to delivery.

**Primary users:** Advertising agency teams (designers, copywriters, account managers, traffic managers, creative directors)
**Primary language:** Hebrew (RTL), with English support
**Platform:** Web (PWA, mobile-first)

---

## 2. Multi-Tenancy Model

- Each advertising agency is an isolated **Tenant** (Agency)
- Every tenant has a unique subdomain: `{slug}.fos.app`
- SuperAdmin manages all agencies from `fos.app/admin`
- Data is fully isolated per agency at DB level (Row-Level Security + application middleware)

---

## 3. User Roles

### 3.1 SuperAdmin (platform-level)
- Manages all agencies
- Can create, activate, deactivate agencies
- Can impersonate any agency for support
- Has access to aggregated platform analytics

### 3.2 AgencyAdmin
- Full control within their agency
- Manages users, roles, clients, workflow rules, SLA, hourly rates
- Can assign AMs to clients
- Can configure Google Workspace integration

### 3.3 Traffic Manager (TM)
- Sees all tasks in the agency studio
- Prioritizes, assigns, and re-assigns tasks
- Manages workload and capacity
- Can perform bulk actions on tasks
- Monitors SLA breaches

### 3.4 Creative Manager (CM)
- Reviews and approves all creative output
- Can approve, reject, or return tasks for revision
- Can delegate approval rights temporarily
- Has read-only access to traffic board

### 3.5 Account Manager (AM)
- Linked explicitly to specific clients only (`AccountManagerClient` table)
- Opens briefs, approves internally, sends to clients, collects feedback
- Sees only tasks for their assigned clients

### 3.6 Designer
- Sees tasks assigned to them
- Must start/stop timer when working
- Uploads files and versions to Google Drive
- Marks tasks ready for review

### 3.7 Copywriter
- Same as Designer but for copy work
- Works in parallel or sequentially depending on task type

### 3.8 External User (Freelancer)
- Sees only tasks directly assigned to them
- Time tracking required
- No access to client list, analytics, or other users' data
- No Google Workspace domain restriction for login

---

## 4. Core Workflow

### 4.1 Status Flow

```
Draft → New → AwaitingTraffic
  → AssignedToCopy → InCopy → ReadyForDesign
  → AssignedToDesigner → InDesign → AwaitingCreativeApproval
  → AwaitingAMApproval → SentToClient → AwaitingClientFeedback
  → Completed → Closed
```

Parallel track (optional): Copy and Design assigned simultaneously.

### 4.2 Revision Tracking
- `revisionRound` counter on Task, incremented on every return-to-fix
- Alert to TM + CM after configurable threshold (default: 3)

### 4.3 Handoff Note
- Mandatory when TM assigns task
- Displayed as banner to assignee

### 4.4 Timer Rules
- Moving out of `InCopy` or `InDesign` auto-stops active timer
- Only one active timer per user across all tasks
- Starting new timer auto-stops previous

---

## 5. Key Entities

See `ERD.md` for full field definitions.

- Agency, User, Role, Permission, Team
- AccountManagerClient (AM↔Client explicit link)
- Client, Brand, Campaign
- TaskType, Task, TaskAssignment
- TaskWatcher (read-only followers)
- TaskStatusHistory, TaskApproval, ApprovalDelegation
- ChatMessage, MessageMention, MessageAttachment
- TaskFile (Google Drive metadata)
- TimeEntry, TimeEntrySummary
- WorkflowRule, WorkflowTransition
- StatusDurationLog
- NotificationEvent, NotificationRecipient, PushSubscription
- SLAPolicy, SLATimer
- SavedView
- CustomField, CustomFieldValue
- AuditLog
- AnalyticsSnapshot, RoleHourlyRate
- UserSkill (Phase 3)

---

## 6. Google Workspace Integration

- **Authentication**: Google Identity Services (OAuth2) — domain-restricted per agency
- **Files**: Google Drive API v3 — auto-folder per task in agency Shared Drive
- **Picker**: Google Picker API — select/upload files from within the app
- **Notifications**: Google Drive push notifications for file changes

---

## 7. Notifications

### Channels
- In-app (real-time via WebSocket)
- Email (via SMTP, MailHog in dev)
- Push (Web Push API via Service Worker)
- Future: Google Chat, Slack

### Events
- New task assigned to me
- My task changed status
- I was mentioned in chat
- Task returned to me for revision
- Pending approval waiting for me
- Task overdue (SLA breach)
- New file uploaded on my task
- My delegation created/expired

### User Controls
- DND hours (per user preference)
- Frequency: immediate / daily digest / weekly digest / off
- Weekly digest: Sunday morning (configurable per agency timezone)

---

## 8. Time Tracking

- Roles that track: Designer, Copywriter, External
- Start/Stop timer per task
- One active timer per user at all times
- All assignees' time aggregated per task (supports mid-task reassignment)
- Time data visibility: see PERMISSIONS.md

---

## 9. Analytics

### Per Task
- Total time (all assignees)
- Time per role
- Revision cycles
- Time in each status (via StatusDurationLog)
- SLA adherence

### Per Client/Brand
- Total tasks, completion rate
- Total hours, average completion time
- Revision rate per client
- Profitability (Phase 3: quotedPrice vs actualCost)

### Per Employee
- Hours this week/month
- Tasks completed
- Revision rate
- Top clients by time

### Personal (My Stats — Phase 3)
- Self-service personal analytics tab in profile

---

## 10. MVP Scope

1. Multi-tenancy + Agency onboarding
2. Google OAuth2 login
3. RBAC + AccountManagerClient + TaskWatcher
4. Clients / Brands / Campaigns
5. Full task brief + workflow
6. Traffic board (Kanban + Workload)
7. Task detail (all tabs)
8. Time tracking (start/stop, live widget)
9. Google Drive integration
10. Chat + notifications
11. Audit log
12. Basic analytics
13. Hebrew RTL UI + English
14. PWA + mobile responsive

---

## 11. Roadmap

| Phase | Features |
|-------|---------|
| 1 (MVP) | Core workflow, auth, tasks, files, chat, notifications, time tracking, basic analytics, Google integration |
| 2 | Clone Task, Bulk Invite, Approval Delegation, Saved Views, StatusDurationLog, Weekly Digest, Bulk Actions |
| 3 | External Users, UserSkills, Quoted Price + Margin, My Stats, Parallel Tracks, Client Portal |
| 4 | Media planning, Finance, Retainers, Full CRM, BI reports |
