import { useEffect, useState } from 'react';
import AdminLayout from '../../../components/admin/layout/AdminLayout';

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  address: string;
  role: string;
}

const emptyUser = {
  id: 0,
  firstName: '',
  lastName: '',
  email: '',
  phoneNumber: '',
  address: '',
  role: '',
  password: ''
};

type UserForm = typeof emptyUser;

export default function AdminUsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Modal state
  const [showAdd, setShowAdd] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const [showDelete, setShowDelete] = useState(false);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [form, setForm] = useState<UserForm>(emptyUser);
  const [formError, setFormError] = useState<string | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  // Fetch users
  const fetchUsers = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await fetch('/api/user');
      if (!res.ok) throw new Error('Failed to fetch users');
      const data = await res.json();
      setUsers(data);
    } catch (err: any) {
      setError(err.message || 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  // Handlers for modals
  const openAdd = () => {
    setForm(emptyUser);
    setFormError(null);
    setShowAdd(true);
  };
  const openEdit = (user: User) => {
    setSelectedUser(user);
    setForm({
      id: user.id,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phoneNumber: user.phoneNumber,
      address: user.address,
      role: user.role,
      password: ''
    });
    setFormError(null);
    setShowEdit(true);
  };
  const openDelete = (id: number) => {
    setDeleteId(id);
    setShowDelete(true);
  };
  const closeModals = () => {
    setShowAdd(false);
    setShowEdit(false);
    setShowDelete(false);
    setFormError(null);
    setDeleteId(null);
    setSelectedUser(null);
  };

  // Form change handler
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  // Add user
  const handleAddUser = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError(null);
    try {
      const res = await fetch('/api/user', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(form),
      });
      if (!res.ok) throw new Error('Failed to add user');
      closeModals();
      await fetchUsers();
    } catch (err: any) {
      setFormError(err.message || 'Unknown error');
    }
  };

  // Edit user
  const handleEditUser = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.id) {
      setFormError("No user selected for update.");
      return;
    }
    setFormError(null);
    try {
      const updateBody: any = {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        phoneNumber: form.phoneNumber,
        address: form.address,
        role: form.role.charAt(0).toUpperCase() + form.role.slice(1).toLowerCase(),
      };
      if (form.password && form.password.trim() !== "") {
        updateBody.password = form.password;
      }
      const res = await fetch(`/api/user/${form.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(updateBody),
      });
      if (!res.ok) throw new Error('Failed to update user');
      closeModals();
      fetchUsers();
    } catch (err: any) {
      setFormError(err.message || 'Unknown error');
    }
  };

  // Delete user
  const handleDeleteUser = async () => {
    console.log('Attempting to delete user with id:', deleteId);
    if (!deleteId) return;
    try {
      const res = await fetch(`/api/user/${deleteId}`, { method: 'DELETE' });
      if (!res.ok) throw new Error('Failed to delete user');
      closeModals();
      fetchUsers();
    } catch (err: any) {
      setFormError(err.message || 'Unknown error');
    }
  };

  return (
    <AdminLayout>
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-semibold text-gray-900">Users</h1>
          <button
            type="button"
            className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700"
            onClick={openAdd}
          >
            Add User
          </button>
        </div>
        <div className="bg-white rounded-lg shadow">
          {loading ? (
            <div className="p-4">Loading users...</div>
          ) : error ? (
            <div className="p-4 text-red-600">{error}</div>
          ) : (
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">First Name</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Last Name</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Phone</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Address</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Role</th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {users.map((user) => (
                    <tr key={user.id}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.id}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.firstName}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.lastName}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.email}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.phoneNumber}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.address}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.role}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <button
                          type="button"
                          className="text-blue-600 hover:text-blue-900 mr-4"
                          onClick={() => openEdit(user)}
                        >
                          Edit
                        </button>
                        <button
                          type="button"
                          className="text-red-600 hover:text-red-900"
                          onClick={() => openDelete(user.id)}
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        {/* Add User Modal */}
        {showAdd && (
          <Modal onClose={closeModals} title="Add User">
            <form onSubmit={handleAddUser} className="space-y-4">
              <input name="firstName" value={form.firstName} onChange={handleChange} placeholder="First Name" className="w-full border p-2 rounded" required />
              <input name="lastName" value={form.lastName} onChange={handleChange} placeholder="Last Name" className="w-full border p-2 rounded" required />
              <input name="email" value={form.email} onChange={handleChange} placeholder="Email" type="email" className="w-full border p-2 rounded" required />
              <input name="phoneNumber" value={form.phoneNumber} onChange={handleChange} placeholder="Phone Number" className="w-full border p-2 rounded" required />
              <input name="address" value={form.address} onChange={handleChange} placeholder="Address" className="w-full border p-2 rounded" required />
              <input name="password" value={form.password} onChange={handleChange} placeholder="Password" type="password" className="w-full border p-2 rounded" required />
              <select name="role" value={form.role} onChange={handleChange} className="w-full border p-2 rounded" required>
                <option value="">Select Role</option>
                <option value="Admin">Admin</option>
                <option value="User">User</option>
              </select>
              {formError && <div className="text-red-600">{formError}</div>}
              <div className="flex justify-end gap-2">
                <button type="button" onClick={closeModals} className="px-4 py-2 bg-gray-300 rounded">Cancel</button>
                <button type="submit" className="px-4 py-2 bg-green-600 text-white rounded">Add</button>
              </div>
            </form>
          </Modal>
        )}

        {/* Edit User Modal */}
        {showEdit && selectedUser && (
          <Modal onClose={closeModals} title="Edit User">
            <form onSubmit={handleEditUser} className="space-y-4">
              <input name="firstName" value={form.firstName} onChange={handleChange} placeholder="First Name" className="w-full border p-2 rounded" required />
              <input name="lastName" value={form.lastName} onChange={handleChange} placeholder="Last Name" className="w-full border p-2 rounded" required />
              <input name="email" value={form.email} onChange={handleChange} placeholder="Email" type="email" className="w-full border p-2 rounded" required />
              <input name="phoneNumber" value={form.phoneNumber} onChange={handleChange} placeholder="Phone Number" className="w-full border p-2 rounded" required />
              <input name="address" value={form.address} onChange={handleChange} placeholder="Address" className="w-full border p-2 rounded" required />
              <input name="password" value={form.password} onChange={handleChange} placeholder="Password (leave blank to keep)" type="password" className="w-full border p-2 rounded" />
              <select name="role" value={form.role} onChange={handleChange} className="w-full border p-2 rounded" required>
                <option value="">Select Role</option>
                <option value="Admin">Admin</option>
                <option value="User">User</option>
              </select>
              {formError && <div className="text-red-600">{formError}</div>}
              <div className="flex justify-end gap-2">
                <button type="button" onClick={closeModals} className="px-4 py-2 bg-gray-300 rounded">Cancel</button>
                <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded">Save</button>
              </div>
            </form>
          </Modal>
        )}

        {/* Delete User Modal */}
        {showDelete && (
          <Modal onClose={closeModals} title="Delete User">
            <div className="space-y-4">
              <p>Are you sure you want to delete this user?</p>
              {formError && <div className="text-red-600">{formError}</div>}
              <div className="flex justify-end gap-2">
                <button type="button" onClick={closeModals} className="px-4 py-2 bg-gray-300 rounded">Cancel</button>
                <button type="button" onClick={handleDeleteUser} className="px-4 py-2 bg-red-600 text-white rounded" disabled={!deleteId}>Delete</button>
              </div>
            </div>
          </Modal>
        )}
      </div>
    </AdminLayout>
  );
}

// Simple Modal component
function Modal({ onClose, title, children }: { onClose: () => void; title: string; children: React.ReactNode; }) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-30">
      <div className="bg-white rounded-lg shadow-lg w-full max-w-md p-6 relative">
        <button onClick={onClose} className="absolute top-2 right-2 text-gray-400 hover:text-gray-600">&times;</button>
        <h2 className="text-xl font-semibold mb-4">{title}</h2>
        {children}
      </div>
    </div>
  );
} 