import { useState, type FC } from 'react';
import AdminLayout from '../../../components/admin/layout/AdminLayout';
import type { User } from '@/types';
import { useUsers } from '@/api/queries/user-queries';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { useForm } from '@tanstack/react-form';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from '@/components/ui/select';
import { CircleX } from 'lucide-react';
import { UserCreateSchema } from '@/schema';
import type { z } from 'zod';

export default function AdminUsersPage() {

  const [modelOpen, setModalOpen] = useState<null | 'add' | 'edit' | 'delete'>(null);

  const handleModalClose = (): void => {
    setModalOpen(null);
  };

  // const openEdit = (user: User) => {
  //   setSelectedUser(user);
  //   setForm({
  //     id: user.id,
  //     firstName: user.firstName,
  //     lastName: user.lastName,
  //     email: user.email,
  //     phoneNumber: user.phoneNumber,
  //     address: user.address,
  //     role: user.role,
  //     password: ''
  //   });
  //   setFormError(null);
  //   setShowEdit(true);
  // };
  // const openDelete = (id: number) => {
  //   setDeleteId(id);
  //   setShowDelete(true);
  // };
  // const closeModals = () => {
  //   setShowAdd(false);
  //   setShowEdit(false);
  //   setShowDelete(false);
  //   setFormError(null);
  //   setDeleteId(null);
  //   setSelectedUser(null);
  // };

  // // Form change handler
  // const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
  //   setForm({ ...form, [e.target.name]: e.target.value });
  // };

  // // Add user
  // const handleAddUser = async (e: React.FormEvent) => {
  //   e.preventDefault();
  //   setFormError(null);
  //   try {
  //     const res = await fetch('/api/user', {
  //       method: 'POST',
  //       headers: { 'Content-Type': 'application/json' },
  //       body: JSON.stringify(form),
  //     });
  //     if (!res.ok) throw new Error('Failed to add user');
  //     closeModals();
  //     // await fetchUsers();
  //   } catch (err: any) {
  //     setFormError(err.message || 'Unknown error');
  //   }
  // };

  // // Edit user
  // const handleEditUser = async (e: React.FormEvent) => {
  //   e.preventDefault();
  //   if (!id) {
  //     setFormError("No user selected for update.");
  //     return;
  //   }
  //   setFormError(null);
  //   try {
  //     const updateBody: any = {
  //       firstName: firstName,
  //       lastName: lastName,
  //       email: email,
  //       phoneNumber: phoneNumber,
  //       address: address,
  //       role: role.charAt(0).toUpperCase() + role.slice(1).toLowerCase(),
  //     };
  //     if (password && password.trim() !== "") {
  //       updateBody.password = password;
  //     }
  //     const res = await fetch(`/api/user/${id}`, {
  //       method: 'PUT',
  //       headers: { 'Content-Type': 'application/json' },
  //       body: JSON.stringify(updateBody),
  //     });
  //     if (!res.ok) throw new Error('Failed to update user');
  //     closeModals();
  //     // fetchUsers();
  //   } catch (err: any) {
  //     setFormError(err.message || 'Unknown error');
  //   }
  // };

  // // Delete user
  // const handleDeleteUser = async () => {
  //   console.log('Attempting to delete user with id:', deleteId);
  //   if (!deleteId) return;
  //   try {
  //     const res = await fetch(`/api/user/${deleteId}`, { method: 'DELETE' });
  //     if (!res.ok) throw new Error('Failed to delete user');
  //     closeModals();
  //     fetchUsers();
  //   } catch (err: any) {
  //     setFormError(err.message || 'Unknown error');
  //   }
  // };

  return (
    <AdminLayout>
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-semibold text-gray-900">Users</h1>
          <button
            type="button"
            className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700"
            onClick={() => { setModalOpen('add'); }}
          >
            Add User
          </button>
        </div>
        <div className="bg-white rounded-lg shadow">
          <div className="overflow-x-auto">
            <table className="w-full divide-y divide-gray-200">
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
              {/* <tbody className="bg-white divide-y divide-gray-200">
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
                </tbody> */}
              <UserRows
              // editHandler={handleEditUser} deleteHandler={handleDeleteUser}
              />
            </table>
          </div>
        </div>
        <AddUserModal isOpen={modelOpen === 'add'} closeHandler={handleModalClose} />
        {/* Add User Modal */}
        {/* {showAdd && (
          <Modal onClose={closeModals} title="Add User">
            <form onSubmit={handleAddUser} className="space-y-4">
              <input name="firstName" value={firstName} onChange={handleChange} placeholder="First Name" className="w-full border p-2 rounded" required />
              <input name="lastName" value={lastName} onChange={handleChange} placeholder="Last Name" className="w-full border p-2 rounded" required />
              <input name="email" value={email} onChange={handleChange} placeholder="Email" type="email" className="w-full border p-2 rounded" required />
              <input name="phoneNumber" value={phoneNumber} onChange={handleChange} placeholder="Phone Number" className="w-full border p-2 rounded" required />
              <input name="address" value={address} onChange={handleChange} placeholder="Address" className="w-full border p-2 rounded" required />
              <input name="password" value={password} onChange={handleChange} placeholder="Password" type="password" className="w-full border p-2 rounded" required />
              <select name="role" value={role} onChange={handleChange} className="w-full border p-2 rounded" required>
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
        )} */}

        {/* Edit User Modal */}
        {/* {showEdit && selectedUser && (
          <Modal onClose={closeModals} title="Edit User">
            <form onSubmit={handleEditUser} className="space-y-4">
              <input name="firstName" value={firstName} onChange={handleChange} placeholder="First Name" className="w-full border p-2 rounded" required />
              <input name="lastName" value={lastName} onChange={handleChange} placeholder="Last Name" className="w-full border p-2 rounded" required />
              <input name="email" value={email} onChange={handleChange} placeholder="Email" type="email" className="w-full border p-2 rounded" required />
              <input name="phoneNumber" value={phoneNumber} onChange={handleChange} placeholder="Phone Number" className="w-full border p-2 rounded" required />
              <input name="address" value={address} onChange={handleChange} placeholder="Address" className="w-full border p-2 rounded" required />
              <input name="password" value={password} onChange={handleChange} placeholder="Password (leave blank to keep)" type="password" className="w-full border p-2 rounded" />
              <select name="role" value={role} onChange={handleChange} className="w-full border p-2 rounded" required>
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
        )} */}

        {/* Delete User Modal */}
        {/* {showDelete && (
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
        )} */}
      </div>
    </AdminLayout>
  );
}

// Simple Modal component
function Modal({ onClose, title, children }: { onClose: () => void; title: string; children: React.ReactNode; }) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center backdrop-blur-lg bg-opacity-30">
      <div className="bg-white rounded-lg shadow-lg w-full max-w-md p-6 relative">
        <Button onClick={onClose} size='icon' className="absolute top-2 right-2">
          <CircleX size={16} />
        </Button>
        <h2 className="text-xl font-semibold mb-4">{title}</h2>
        {children}
      </div>
    </div>
  );
}

type UserRowsProps = {
  editHandler?: (user: User) => void,
  deleteHandler?: (user: User) => void,
};

const UserRows: FC<UserRowsProps> = ({ editHandler, deleteHandler }) => {
  const { data: users, isLoading, error } = useUsers();
  const tbodyStyle = 'bg-white divide-y divide-gray-200';

  if (isLoading) {
    return (
      <tbody className={cn(tbodyStyle, 'bg-slate-100 h-20 animate-pulse')}>
        <tr>
          <td className='w-full h-full'>&nbsp;</td>
          <td className='w-full h-full'>&nbsp;</td>
          <td className='w-full h-full'>&nbsp;</td>
          <td className='w-full h-full'>&nbsp;</td>
          <td className='w-full h-full'>&nbsp;</td>
          <td className='w-full h-full'>&nbsp;</td>
          <td className='w-full h-full'>&nbsp;</td>
          <td className='w-full h-full'>&nbsp;</td>
        </tr>
      </tbody>
    );
  }

  if (error || !users) {
    return (
      <tbody className={cn(tbodyStyle)}>
        <p>{'Something went wrong'}</p>
      </tbody>
    );
  }

  return (
    <tbody className={tbodyStyle}>
      {users.map((user) => (
        <tr key={user.id}>
          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.id}</td>
          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.firstName}</td>
          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.lastName}</td>
          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.email}</td>
          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.phoneNumber}</td>
          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.address}</td>
          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{user.role}</td>
          {editHandler && deleteHandler
            ? (
              <>
                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <button
                    type="button"
                    className="text-blue-600 hover:text-blue-900 mr-4"
                    onClick={() => { editHandler(user); }}
                  >
                    Edit
                  </button>
                  <button
                    type="button"
                    className="text-red-600 hover:text-red-900"
                    onClick={() => { deleteHandler(user); }}
                  >
                    Delete
                  </button>
                </td>
              </>
            )
            : null}
        </tr>
      ))}
    </tbody>
  );
};

type AddUserModalProps = {
  isOpen: boolean;
  closeHandler: () => void;
};

const AddUserModal: FC<AddUserModalProps> = ({ isOpen, closeHandler }) => {

  // eslint-disable-next-line @typescript-eslint/unbound-method
  const { Field, Subscribe, handleSubmit: handleFormSubmit, reset } = useForm({
    defaultValues: {
      firstName: 'John',
      lastName: 'Doe',
      email: 'johndoe@email.com',
      phoneNumber: '12345678',
      address: 'Applestreet 12',
      city: 'Springfield',
      zipCode: 1234,
      country: 'United State of America',
      role: 'Customer',
      password: 'password123'
    } as z.infer<typeof UserCreateSchema>,
    validators: {
      onChange: UserCreateSchema
    },
    onSubmit: ({ value: user }) => {
      // Do something with form data
      console.log(user);
    },
  },);

  if (!isOpen) {
    return null;
  }

  return (
    <Modal onClose={closeHandler} title="Add User">
      <form
        onSubmit={(e) => {
          e.preventDefault();
          e.stopPropagation();
          void handleFormSubmit();
        }}
        className="space-y-4">
        <Field
          name='firstName'
          children={(field) => (
            <>
              <Label htmlFor='firstName'>Firstname</Label>
              <Input id='firstName' type='text' value={field.state.value} onChange={(e) => { field.handleChange(e.target.value); }} />
            </>
          )}
        />
        <Field
          name='lastName'
          children={(field) => (
            <>
              <Label htmlFor='lastName'>Lastname</Label>
              <Input id='lastName' type='text' value={field.state.value} onChange={(e) => { field.handleChange(e.target.value); }} />
            </>
          )}
        />
        <Field
          name='email'
          children={(field) => (
            <>
              <Label htmlFor='email'>Email</Label>
              <Input id='email' type='email' value={field.state.value} onChange={(e) => { field.handleChange(e.target.value); }} />
            </>
          )}
        />
        <Field
          name='phoneNumber'
          children={(field) => (
            <>
              <Label htmlFor='phoneNumber'>Phone Number</Label>
              <Input id='phoneNumber' type='tel' value={field.state.value} onChange={(e) => { field.handleChange(e.target.value); }} />
            </>
          )}
        />
        <Field
          name='address'
          children={(field) => (
            <>
              <Label htmlFor='address'>Address</Label>
              <Input id='address' type='text' value={field.state.value} onChange={(e) => { field.handleChange(e.target.value); }} />
            </>
          )}
        />
        <Field
          name='city'
          children={(field) => (
            <>
              <Label htmlFor='city'>City</Label>
              <Input id='city' type='text' value={field.state.value} onChange={(e) => { field.handleChange(e.target.value); }} />
            </>
          )}
        />
        <Field
          name='zipCode'
          children={(field) => (
            <>
              <Label htmlFor='zipCode'>Address</Label>
              <Input id='zipCode' type='text' value={field.state.value} onChange={(e) => { field.handleChange(parseInt(e.target.value, 10)); }} />
            </>
          )}
        />
        <Field
          name='country'
          children={(field) => (
            <>
              <Label htmlFor='country'>City</Label>
              <Input id='country' type='text' value={field.state.value} onChange={(e) => { field.handleChange(e.target.value); }} />
            </>
          )}
        />
        <Field
          name='role'
          children={(field) => (
            <>
              <Label htmlFor='role'>Role:</Label>
              <Select onValueChange={(e) => { field.handleChange(e as z.infer<typeof UserCreateSchema>['role']); }}>
                <SelectTrigger className="w-full">
                  <SelectValue defaultValue={field.state.value} placeholder='Select a role' />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectLabel>Roles</SelectLabel>
                    <SelectItem value="Customer">Customer</SelectItem>
                    <SelectItem value="Admin">Administrator</SelectItem>
                    <SelectItem value="DeliveryPerson">Delivery Person</SelectItem>
                  </SelectGroup>
                </SelectContent>
              </Select>
            </>
          )}
        />
        <Field
          name='password'
          children={(field) => (
            <>
              <Label htmlFor='password'>Password</Label>
              <Input id='password' type='password' value={field.state.value} onChange={(e) => { field.handleChange(e.target.value); }} />
            </>
          )}
        />
        <Subscribe
          selector={(state) => [state.canSubmit, state.isSubmitting]}
          children={([canSubmit, isSubmitting]) => (
            <div className="flex justify-between gap-2">
              <Button type='submit' disabled={!canSubmit} size={'lg'} >
                {isSubmitting ? '...' : 'Submit'}
              </Button>
              <Button type="reset" variant={'destructive'} onClick={() => { reset(); }} size={'lg'}>
                Reset
              </Button>
            </div>
          )}
        />
      </form>
    </Modal >
  );
};