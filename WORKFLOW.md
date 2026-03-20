# FOS — Workflow Rules & Status Machine

---

## Status Definitions

| Status | Hebrew | Description |
|--------|--------|-------------|
| DRAFT | טיוטה | Brief being written, not submitted |
| NEW | חדש | Brief submitted, awaiting traffic |
| AWAITING_TRAFFIC | ממתין לטראפיק | In traffic queue for prioritization |
| AWAITING_BRIEF_COMPLETION | ממתין להשלמת בריף | TM flagged brief as incomplete |
| ASSIGNED_TO_COPY | הוקצה לקופי | Copywriter assigned |
| IN_COPY | בעבודת קופי | Timer active, copy in progress |
| READY_FOR_DESIGN | מוכן לעיצוב | Copy done, awaiting designer assignment |
| ASSIGNED_TO_DESIGNER | הוקצה למעצב | Designer assigned |
| IN_DESIGN | בעיצוב | Timer active, design in progress |
| AWAITING_CREATIVE_APPROVAL | ממתין לאישור קריאייטיב | Awaiting CM review |
| AWAITING_BRIEF_CLARIFICATION | ממתין להבהרת בריף | CM needs more info from AM |
| RETURN_TO_DESIGN | חזר לתיקון עיצוב | CM/AM returned to designer |
| RETURN_TO_COPY | חזר לתיקון קופי | CM returned to copywriter |
| AWAITING_AM_APPROVAL | ממתין לאישור מנהל לקוח | Awaiting AM internal approval |
| SENT_TO_CLIENT | נשלח ללקוח | AM sent to external client |
| AWAITING_CLIENT_FEEDBACK | ממתין לפידבק לקוח | Waiting for client response |
| RETURN_FROM_CLIENT | חזר מתיקוני לקוח | Client requested changes |
| COMPLETED | הושלם | Client approved |
| CLOSED | נסגר | Officially closed |
| CANCELLED | בוטל | Cancelled |
| ON_HOLD | מושהה | Paused |

---

## Legal Transitions

### From DRAFT
| To | Who | Requires |
|----|-----|---------|
| NEW | AM (creator) | — |
| CANCELLED | AM, AgencyAdmin | note |

### From NEW
| To | Who | Requires |
|----|-----|---------|
| AWAITING_TRAFFIC | AM | — |
| DRAFT | AM | — |
| CANCELLED | AM, AgencyAdmin | note |

### From AWAITING_TRAFFIC
| To | Who | Requires |
|----|-----|---------|
| AWAITING_BRIEF_COMPLETION | TM | note explaining what is missing |
| ASSIGNED_TO_COPY | TM | copywriterId, handoffNote |
| ASSIGNED_TO_DESIGNER | TM | designerId, handoffNote |
| ON_HOLD | TM, AgencyAdmin | note |
| CANCELLED | TM, AgencyAdmin | note |

### From AWAITING_BRIEF_COMPLETION
| To | Who | Requires |
|----|-----|---------|
| AWAITING_TRAFFIC | AM | — |
| CANCELLED | AM, AgencyAdmin | note |

### From ASSIGNED_TO_COPY
| To | Who | Requires |
|----|-----|---------|
| IN_COPY | Copywriter (assigned) | — (starts timer automatically) |
| AWAITING_TRAFFIC | TM | note (reassign) |

### From IN_COPY
| To | Who | Requires |
|----|-----|---------|
| READY_FOR_DESIGN | Copywriter | — (stops timer) |
| AWAITING_TRAFFIC | TM | note (urgent reassign) |
| ON_HOLD | TM | note |

### From READY_FOR_DESIGN
| To | Who | Requires |
|----|-----|---------|
| ASSIGNED_TO_DESIGNER | TM | designerId, handoffNote |

### From ASSIGNED_TO_DESIGNER
| To | Who | Requires |
|----|-----|---------|
| IN_DESIGN | Designer (assigned) | — (starts timer) |
| AWAITING_TRAFFIC | TM | note (reassign) |

### From IN_DESIGN
| To | Who | Requires |
|----|-----|---------|
| AWAITING_CREATIVE_APPROVAL | Designer | — (stops timer) |
| AWAITING_TRAFFIC | TM | note |
| ON_HOLD | TM | note |

### From AWAITING_CREATIVE_APPROVAL
| To | Who | Requires |
|----|-----|---------|
| AWAITING_AM_APPROVAL | CM, delegatee | — |
| AWAITING_BRIEF_CLARIFICATION | CM | note explaining what info is needed |
| RETURN_TO_DESIGN | CM | rejectionReason, optional designerId |
| RETURN_TO_COPY | CM | rejectionReason, optional copywriterId |

### From AWAITING_BRIEF_CLARIFICATION
| To | Who | Requires |
|----|-----|---------|
| AWAITING_CREATIVE_APPROVAL | AM | clarification note |

### From RETURN_TO_DESIGN
| To | Who | Requires |
|----|-----|---------|
| IN_DESIGN | Designer | — (timer starts, revisionRound++) |
| ASSIGNED_TO_DESIGNER | TM | designerId (if reassigning) |

### From RETURN_TO_COPY
| To | Who | Requires |
|----|-----|---------|
| IN_COPY | Copywriter | — (timer starts, revisionRound++) |
| ASSIGNED_TO_COPY | TM | copywriterId (if reassigning) |

### From AWAITING_AM_APPROVAL
| To | Who | Requires |
|----|-----|---------|
| SENT_TO_CLIENT | AM | — |
| RETURN_TO_DESIGN | AM | note |
| RETURN_TO_COPY | AM | note |

### From SENT_TO_CLIENT
| To | Who | Requires |
|----|-----|---------|
| AWAITING_CLIENT_FEEDBACK | AM | — |
| AWAITING_AM_APPROVAL | AM | note (pulled back) |

### From AWAITING_CLIENT_FEEDBACK
| To | Who | Requires |
|----|-----|---------|
| COMPLETED | AM | — |
| RETURN_FROM_CLIENT | AM | clientFeedbackNote |

### From RETURN_FROM_CLIENT
| To | Who | Requires |
|----|-----|---------|
| ASSIGNED_TO_DESIGNER | TM | designerId, revisionRound++ |
| ASSIGNED_TO_COPY | TM | copywriterId, revisionRound++ |
| AWAITING_AM_APPROVAL | AM | note (minor change handled internally) |

### From COMPLETED
| To | Who | Requires |
|----|-----|---------|
| CLOSED | AM, AgencyAdmin | — |

### From ON_HOLD
| To | Who | Requires |
|----|-----|---------|
| AWAITING_TRAFFIC | TM | note |
| CANCELLED | AgencyAdmin | note |

---

## Workflow Rules Engine

Each WorkflowRule record defines:

```
{
  agencyId
  fromStatus
  toStatus
  allowedRoles: Role[]
  requiresNote: boolean
  requiresFile: boolean
  requiresNextAssignee: boolean
  autoStopTimer: boolean        // stops assignee timer on transition
  revisionIncrement: boolean    // increments revisionRound
  notificationTargets: NotificationTarget[]  // who gets notified
  slaResets: boolean            // whether SLA clock resets
}
```

Default rules are seeded for every new agency. AgencyAdmin can customize.

---

## SLA Policies

Default SLA thresholds (configurable per agency):

| Stage | Default SLA |
|-------|------------|
| Brief → Traffic assignment | 4 hours |
| Traffic → Copy/Design assignment | 2 hours |
| Copy work | 24 hours |
| Design work | 48 hours |
| Creative approval | 8 hours |
| AM approval | 4 hours |
| Client feedback | 72 hours |

SLA breach: task marked red, alert to TM and direct manager.

---

## Revision Alert Rule

When `revisionRound` reaches threshold (default 3):
- Notification to Traffic Manager
- Notification to Creative Manager
- Visual alert badge on task (red exclamation)
- Task added to "Needs Attention" section in dashboard

---

## Auto-Timer Stop Rules

Timer auto-stops when:
- Task status moves out of IN_COPY or IN_DESIGN (any transition)
- User starts a timer on a different task
- User marks themselves unavailable
