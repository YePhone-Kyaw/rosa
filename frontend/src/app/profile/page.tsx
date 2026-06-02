"use client";

import ProtectedRoute from "@/components/ProtectedRoute";
import api from "@/lib/api";
import { useStore } from "@/store/useStore";
import axios from "axios";
import { useState } from "react";

export default function ProfilePage() {
  const { user, setUser } = useStore();

  const [name, setName] = useState(user?.name);
  const [email, setEmail] = useState(user?.email);
  const [profilePicture, setProfilePicture] = useState(user?.profilePicture);
  const [edit, setEdit] = useState(false);
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [profileMessage, setProfileMessage] = useState("");

  const handleSave = async () => {
    try {
      const response = await api.put("/user/profile", { name, email });
      const data = response.data;
      setUser(data);

      if (currentPassword && newPassword) {
        await api.put('/user/password', { currentPassword, newPassword });
        setCurrentPassword('');
        setNewPassword('');
      }

      setProfileMessage("Profile updated successfully");
      setEdit(false);
    } catch (error) {
      if (axios.isAxiosError(error)) {
        setProfileMessage(error.response?.data.message);
      }
    }
  };

  return (
    <ProtectedRoute>
      <div className="max-w-2xl mx-auto">
        <h1 className="text-3xl font-bold text-gray-900 mb-8">Profile</h1>

        <div className="bg-white border rounded-xl p-6 mb-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-bold text-gray-900">
              Account Information
            </h2>
            {!edit && (
              <button
                onClick={() => {
                    setName(user?.name);
                    setEmail(user?.email);
                    setEdit(true);
                }}
                className="text-rose-600 text-sm font-medium hover:underline"
              >
                Edit
              </button>
            )}
          </div>

          {profileMessage && (
            <p className="text-green-600 text-sm mb-4 bg-green-50 px-4 py-2 rounded-lg">
              {profileMessage}
            </p>
          )}

          {edit ? (
            <div className="space-y-4">
              <div className="flex items-center gap-4 mb-4">
                <div className="w-20 h-20 bg-gray-100 rounded-full flex items-center justify-center">
                  <span className="text-3xl">👤</span>
                </div>
                <button
                  disabled
                  className="text-sm text-gray-400 cursor-not-allowed"
                >
                  Upload Photo (Coming Soon)
                </button>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Name
                </label>
                <input
                  type="text"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-gray-900 focus:outline-none focus:ring-2 focus:ring-rose-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Email
                </label>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-gray-900 focus:outline-none focus:ring-2 focus:ring-rose-500"
                />
              </div>

              <div className="border-t pt-4 mt-4">
                <h3 className="text-sm font-bold text-gray-900 mb-3">
                  Change Password
                </h3>
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Current Password
                    </label>
                    <input
                      type="password"
                      value={currentPassword}
                      onChange={(e) => setCurrentPassword(e.target.value)}
                      className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-gray-900 focus:outline-none focus:ring-2 focus:ring-rose-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      New Password
                    </label>
                    <input
                      type="password"
                      value={newPassword}
                      onChange={(e) => setNewPassword(e.target.value)}
                      className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-gray-900 focus:outline-none focus:ring-2 focus:ring-rose-500"
                    />
                  </div>
                </div>
              </div>

              <div className="flex gap-3 pt-2">
                <button
                  onClick={handleSave}
                  className="bg-rose-600 text-white px-6 py-2.5 rounded-lg font-medium hover:bg-rose-700 transition"
                >
                  Save Changes
                </button>
                <button
                  onClick={() => {
                    setEdit(false);
                    setName(user?.name || "");
                    setEmail(user?.email || "");
                    setCurrentPassword("");
                    setNewPassword("");
                    setProfileMessage("");
                  }}
                  className="border border-gray-300 text-gray-700 px-6 py-2.5 rounded-lg font-medium hover:bg-gray-50 transition"
                >
                  Cancel
                </button>
              </div>
            </div>
          ) : (
            <div className="space-y-3">
                <div className="flex flex-col items-start gap-4">
              <div className="w-20 h-20 bg-gray-100 rounded-full flex items-center justify-center">
                <span className="text-3xl">👤</span>
              </div>
              <div>
                <p className="text-sm text-gray-500">Name</p>
                <p className="text-gray-900">{user?.name}</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Email</p>
                <p className="text-gray-900">{user?.email}</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Password</p>
                <p className="text-gray-900">••••••••</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Role</p>
                <p className="text-gray-900 capitalize">{user?.role}</p>
              </div>
            </div>
                </div>
          )}
        </div>
      </div>
    </ProtectedRoute>
  );
}
