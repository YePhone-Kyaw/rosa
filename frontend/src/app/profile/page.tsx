"use client";

import ProtectedRoute from "@/components/ProtectedRoute";
import api from "@/lib/api";
import { useStore } from "@/store/useStore";
import axios from "axios";
import Image from "next/image";
import { useEffect, useRef, useState } from "react";

export default function ProfilePage() {
  const { user, setUser } = useStore();

  const [name, setName] = useState(user?.name);
  const [email, setEmail] = useState(user?.email);
  const [edit, setEdit] = useState(false);
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [profileMessage, setProfileMessage] = useState("");
  const [preview, setPreview] = useState<string | null>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [removePhoto, setRemovePhoto] = useState(false);
  const [saving, setSaving] = useState(false);
  const [showPhotoMenu, setShowPhotoMenu] = useState(false);
  const photoMenuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClick = (e: MouseEvent) => {
      if (
        photoMenuRef.current &&
        !photoMenuRef.current.contains(e.target as Node)
      ) {
        setShowPhotoMenu(false);
      }
    };
    document.addEventListener("click", handleClick);
    return () => document.removeEventListener("click", handleClick);
  }, []);

  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setSelectedFile(file);
    setRemovePhoto(false);

    const reader = new FileReader();
    reader.onload = (e) => setPreview(e.target?.result as string);
    reader.readAsDataURL(file);
  };

  const handleRemovePhoto = () => {
    setRemovePhoto(true);
    setPreview(null);
    setSelectedFile(null);
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      const response = await api.put("/user/profile", { name, email });
      let updatedUser = response.data;

      if (selectedFile) {
        const formData = new FormData();
        formData.append("profileImage", selectedFile);
        const uploadResponse = await api.post(
          "/user/profile/picture",
          formData,
        );
        updatedUser = {
          ...updatedUser,
          profilePicture: uploadResponse.data.profilePicture,
        };
      }

      if (removePhoto) {
        await api.delete("/user/profile/picture");
        updatedUser = { ...updatedUser, profilePicture: null };
      }

      if (currentPassword && newPassword) {
        await api.put("/user/password", { currentPassword, newPassword });
        setCurrentPassword("");
        setNewPassword("");
      }

      setUser(updatedUser);
      setProfileMessage("Profile updated successfully");
      setEdit(false);
      setSelectedFile(null);
      setPreview(null);
      setRemovePhoto(false);
    } catch (error) {
      if (axios.isAxiosError(error)) {
        setProfileMessage(error.response?.data.message);
      }
    }
    setSaving(false);
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
              <div className="relative mb-4" ref={photoMenuRef}>
                <div
                  onClick={() => setShowPhotoMenu(!showPhotoMenu)}
                  className="relative w-20 h-20 rounded-full cursor-pointer"
                >
                  <div className="w-20 h-20 bg-gray-100 rounded-full overflow-hidden">
                    {preview ? (
                      <Image
                        src={preview}
                        alt="Preview"
                        width={80}
                        height={80}
                        className="w-full h-full object-cover"
                      />
                    ) : !removePhoto && user?.profilePicture ? (
                      <Image
                        src={user.profilePicture}
                        alt="Profile"
                        width={80}
                        height={80}
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center">
                        <span className="text-3xl">👤</span>
                      </div>
                    )}
                  </div>
                  <div className="absolute inset-0 bg-black/50 rounded-full flex items-center justify-center">
                    <span className="text-white text-xs font-medium">Edit</span>
                  </div>
                </div>

                {showPhotoMenu && (
                  <div className="absolute left-0 top-full mt-2 bg-white border rounded-xl shadow-lg z-50 w-40">
                    <label className="block px-4 py-3 text-sm text-gray-700 hover:bg-gray-50 rounded-t-xl cursor-pointer">
                      Choose Photo
                      <input
                        type="file"
                        accept="image/*"
                        onChange={(e) => {
                          handleFileSelect(e);
                          setShowPhotoMenu(false);
                        }}
                        className="hidden"
                      />
                    </label>
                    {(user?.profilePicture || preview) && !removePhoto && (
                      <button
                        onClick={() => {
                          handleRemovePhoto();
                          setShowPhotoMenu(false);
                        }}
                        className="block w-full text-left px-4 py-3 text-sm text-red-500 hover:bg-gray-50 rounded-b-xl"
                      >
                        Remove Photo
                      </button>
                    )}
                  </div>
                )}
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
                  disabled={saving}
                  className="bg-rose-600 text-white px-6 py-2.5 rounded-lg font-medium hover:bg-rose-700 transition disabled:opacity-50"
                >
                  {saving ? "Saving..." : "Save Changes"}
                </button>
                <button
                  onClick={() => {
                    setEdit(false);
                    setName(user?.name || "");
                    setEmail(user?.email || "");
                    setCurrentPassword("");
                    setNewPassword("");
                    setProfileMessage("");
                    setPreview(null);
                    setSelectedFile(null);
                    setRemovePhoto(false);
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
                <div className="w-20 h-20 bg-gray-100 rounded-full flex items-center justify-center overflow-hidden">
                  {user?.profilePicture ? (
                    <Image
                      src={user.profilePicture}
                      alt="Profile"
                      width={80}
                      height={80}
                      className="w-full h-full object-cover"
                    />
                  ) : (
                    <span className="text-3xl">👤</span>
                  )}
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
