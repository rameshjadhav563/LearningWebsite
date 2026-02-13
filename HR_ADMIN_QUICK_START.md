# 🚀 HR ADMIN QUICK START GUIDE

## What You Can Do as HR Admin

### ✅ **Full CRUD Operations**
- **C**reate new users (Employees, Managers, HR Admins)
- **R**ead/View all user information
- **U**pdate user details, roles, and passwords
- **D**elete users permanently

### ✅ **Role Management**
- Assign roles: Employee, Manager, HR Admin
- Change roles with one click
- Auto-handle team member reassignment

---

## Quick Actions

### 1. **Create a New User**
```
HR Dashboard → Create New User
```
- Required: Username, Password, Role
- Optional: Full Name, Email, Manager

### 2. **View All Users**
```
HR Dashboard → Manage All Users
```
- Search by username/name/email
- Filter by role
- Quick actions on each row

### 3. **Edit User**
```
Manage Users → Edit button (yellow)
```
- Change any user information
- Update role
- Reset password (optional)

### 4. **Delete User**
```
Manage Users → Delete button (red) → Confirm
```
- Shows deletion warnings
- Requires checkbox confirmation
- Permanent deletion

### 5. **Change Role**
```
Manage Users → Role dropdown → Select role
```
- Set as Employee
- Set as Manager
- Set as HR Admin

### 6. **View Details**
```
Manage Users → View button (blue)
```
- Complete user profile
- Learning assignments
- Team members (if manager)
- Statistics

---

## URL Routes

| Page | URL | Description |
|------|-----|-------------|
| Dashboard | `/HR/Index` | Main HR admin dashboard |
| Manage Users | `/HR/ManageUsers` | User management table |
| Create User | `/HR/CreateUser` | Add new user form |
| Edit User | `/HR/EditUser/{id}` | Edit user form |
| Delete User | `/HR/DeleteUser/{id}` | Deletion confirmation |
| User Details | `/HR/UserDetails/{id}` | Complete profile view |

---

## Test Account

```
Username: hr1
Password: password
Role: HR Admin
Access: Full administrative rights
```

---

## Example Workflow

### Create an Employee Under a Manager

1. **Login as HR:**
   - Go to `/Account/Login`
   - Enter `hr1` / `password`

2. **Navigate:**
   - Click "Create New User"

3. **Fill Form:**
   - Username: `employee6`
   - Password: `password123`
   - Full Name: `John Doe`
   - Email: `john@company.com`
   - Role: `Employee`
   - Manager: `manager1`

4. **Submit:**
   - Click "Create User"

5. **Verify:**
   - User appears in "Manage All Users"
   - Employee shows under manager1's team

### Change a User's Role

1. **Go to Manage Users**
2. **Find user:** Use search or scroll
3. **Click Role dropdown**
4. **Select new role:** e.g., "Set as Manager"
5. **Confirm:** Click OK
6. **Done!** Role changed instantly

### Delete a Test User

1. **Go to Manage Users**
2. **Click Delete (trash icon)**
3. **Review warnings:** Assignments, team members
4. **Check confirmation box**
5. **Click "Yes, Delete This User"**
6. **User removed permanently**

---

## Dashboard Overview

### Stats Cards
- **Total Users:** All users in system
- **Employees:** Users with Employee role
- **Managers:** Users with Manager role
- **HR Admins:** Users with HR role

### Quick Actions
- ✅ Create New User
- ✅ Manage All Users
- ✅ View Roles
- ✅ Recent Activity

### Recent Users Table
- Shows last 10 users
- Quick view/edit actions
- Role badges (color-coded)

---

## Manage Users Features

### Search
- Real-time search
- Searches: username, full name, email

### Filter by Role
- All Roles
- Employee only
- Manager only
- HR Admin only

### Actions Per User
- 👁️ **View** - User details
- ✏️ **Edit** - Modify user
- 🗑️ **Delete** - Remove user
- 🔄 **Role** - Change role dropdown

---

## Important Notes

### ⚠️ Delete Warnings
- **Permanent action** - Cannot be undone
- **Assignments deleted** - All learning assignments removed
- **Team unassigned** - If manager, team members lose manager

### ✅ Best Practices
- Always verify before deleting
- Use edit to change roles
- Test with demo accounts first
- Check user details before modifications

### 🔒 Security
- Password hashing automatic
- Anti-forgery protection enabled
- HR-only authorization enforced
- Username uniqueness validated

---

## Common Tasks

### Task: Add Multiple Employees
1. Create User → Employee1 → Submit
2. Create User → Employee2 → Submit
3. Create User → Employee3 → Submit
4. Verify in Manage Users

### Task: Promote Employee to Manager
1. Manage Users → Find employee
2. Role dropdown → "Set as Manager"
3. Confirm
4. Done! Now can have team

### Task: Bulk Role Check
1. Manage Users
2. Filter → Employee
3. Review all employees
4. Change roles as needed

---

## Troubleshooting

### Can't Create User
- ✅ Check username is unique
- ✅ Verify password length (min 6)
- ✅ Select a role

### Can't Delete User
- ✅ Logged in as HR?
- ✅ Check confirmation box?
- ✅ User exists?

### Role Not Changing
- ✅ Confirm dialog clicked?
- ✅ Check current role after change
- ✅ Refresh page

---

## Keyboard Shortcuts

- **Ctrl+F** - Focus search box (in browser)
- **Tab** - Navigate form fields
- **Enter** - Submit form
- **Esc** - Close modals

---

## Quick Reference

```
Login → HR Dashboard → Choose Action

Actions:
├─ Create User (green button)
├─ Manage Users (blue button)
├─ View Details (info icon)
├─ Edit User (yellow icon)
├─ Delete User (red icon)
└─ Change Role (dropdown)
```

---

## ✅ You're Ready!

Everything is set up. Start testing:

1. **Stop app:** `Shift+F5`
2. **Start app:** `F5`
3. **Login:** `hr1` / `password`
4. **Test:** Create, Edit, Delete users

**Good luck!** 🎉
