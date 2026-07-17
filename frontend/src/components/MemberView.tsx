import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import type { MemberProfile } from '../types/member';
import { User, Users, ShieldAlert, CheckCircle2, HeartPulse, Sparkles, CreditCard, Clock } from 'lucide-react';

const API_URL = 'http://localhost:5264/api';

export const MemberView: React.FC = () => {
  const { token, user } = useAuth();
  const [profile, setProfile] = useState<MemberProfile | null>(null);
  const [allMembers, setAllMembers] = useState<MemberProfile[]>([]);

  // Form States - Membership Purchase
  const [subType, setSubType] = useState('Monthly');
  const [subDuration, setSubDuration] = useState(1);

  // Form States - Profile updates
  const [medNotes, setMedNotes] = useState('');
  const [emerName, setEmerName] = useState('');
  const [emerPhone, setEmerPhone] = useState('');

  // UI States
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const canSeeAllMembers = user && (user.role === 'Super Admin' || user.role === 'System Owner' || user.role === 'Branch Manager' || user.role === 'Receptionist');

  const fetchMyProfile = async () => {
    try {
      const res = await fetch(`${API_URL}/member/me`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Failed to load profile.');
      const data = await res.json();
      setProfile(data);
      if (data) {
        setMedNotes(data.medicalNotes || '');
        setEmerName(data.emergencyContactName || '');
        setEmerPhone(data.emergencyContactPhone || '');
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const fetchAllMembers = async () => {
    try {
      const res = await fetch(`${API_URL}/member`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Failed to load members.');
      const data = await res.json();
      setAllMembers(data);
    } catch (err: any) {
      setError(err.message);
    }
  };

  useEffect(() => {
    if (token) {
      fetchMyProfile();
      if (canSeeAllMembers) {
        fetchAllMembers();
      }
    }
  }, [token]);

  const handleUpdateProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);
    try {
      const res = await fetch(`${API_URL}/member/me/profile`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          medicalNotes: medNotes,
          emergencyContactName: emerName,
          emergencyContactPhone: emerPhone
        })
      });
      if (!res.ok) throw new Error('Failed to update profile details.');
      const data = await res.json();
      setProfile(data);
      setSuccess('Profile details saved successfully!');
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handlePurchaseMembership = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);
    try {
      const res = await fetch(`${API_URL}/member/me/membership`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          membershipType: subType,
          durationInMonths: Number(subDuration)
        })
      });
      if (!res.ok) throw new Error('Failed to update membership subscription.');
      const data = await res.json();
      setProfile(data);
      setSuccess(`Successfully subscribed to ${subType} Plan!`);
      if (canSeeAllMembers) {
        fetchAllMembers();
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-8">
      {/* Notifications */}
      {success && (
        <div className="p-3.5 bg-emerald-50 border border-emerald-200 text-emerald-700 rounded-xl flex items-center text-sm font-medium">
          <CheckCircle2 className="mr-2 h-5 w-5 flex-shrink-0" />
          {success}
        </div>
      )}

      {error && (
        <div className="p-3.5 bg-red-50 border border-red-200 text-red-700 rounded-xl flex items-center text-sm font-medium">
          <ShieldAlert className="mr-2 h-5 w-5 flex-shrink-0" />
          {error}
        </div>
      )}

      {/* Profile Overview & Membership Tier */}
      {profile && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Member Profile Details & Subscription View */}
          <div className="md:col-span-1 bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-6">
            <div className="flex items-center space-x-4">
              <div className="bg-blue-100 text-blue-700 p-3.5 rounded-2xl">
                <User className="h-6 w-6" />
              </div>
              <div>
                <h3 className="text-lg font-black text-gray-900">{profile.fullName}</h3>
                <p className="text-xs text-gray-400 font-semibold uppercase">{profile.email}</p>
              </div>
            </div>

            {/* Current Membership card */}
            <div className={`p-4 rounded-xl border ${
              profile.isMembershipActive
                ? 'bg-gradient-to-br from-blue-500 to-indigo-600 text-white border-transparent shadow-md'
                : 'bg-gray-50 border-gray-100 text-gray-700'
            }`}>
              <div className="flex justify-between items-start mb-2">
                <span className="text-xs font-bold uppercase tracking-wider">Current Membership</span>
                <Sparkles className="h-4 w-4" />
              </div>
              <p className="text-2xl font-black">{profile.membershipType}</p>
              <p className="text-[11px] opacity-90 mt-1 flex items-center">
                <Clock className="h-3.5 w-3.5 mr-1" />
                {profile.isMembershipActive
                  ? `Active until ${new Date(profile.membershipEndDate).toLocaleDateString()}`
                  : 'No active subscription plan'}
              </p>
            </div>

            {/* Update Profile (Emergency / Medical) */}
            <form onSubmit={handleUpdateProfile} className="space-y-4 pt-2">
              <h4 className="text-sm font-bold text-gray-800 border-b pb-2 flex items-center">
                <HeartPulse className="mr-2 h-4 w-4 text-rose-500" />
                Health & Emergency Details
              </h4>
              <div>
                <label className="block text-xs font-semibold text-gray-500 mb-1">Medical/Fitness Notes</label>
                <textarea
                  value={medNotes}
                  onChange={e => setMedNotes(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500 min-h-[60px]"
                  placeholder="e.g. Asthma, Knee surgery in 2024"
                />
              </div>
              <div>
                <label className="block text-xs font-semibold text-gray-500 mb-1">Emergency Contact Name</label>
                <input
                  type="text"
                  value={emerName}
                  onChange={e => setEmerName(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                  placeholder="e.g. Jane Doe"
                />
              </div>
              <div>
                <label className="block text-xs font-semibold text-gray-500 mb-1">Emergency Phone Number</label>
                <input
                  type="text"
                  value={emerPhone}
                  onChange={e => setEmerPhone(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                  placeholder="e.g. +1 555-0199"
                />
              </div>
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-gray-900 hover:bg-black text-white font-semibold py-2 rounded-lg text-xs transition"
              >
                Save Profile Information
              </button>
            </form>
          </div>

          {/* Membership Tier Purchase */}
          <div className="md:col-span-2 bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-6">
            <h3 className="text-lg font-bold text-gray-800 flex items-center border-b pb-2">
              <CreditCard className="mr-2 text-blue-600" />
              Upgrade Gym Membership Subscriptions
            </h3>
            <p className="text-xs text-gray-500 leading-relaxed">
              Unlock access to specialized gyms, booking discounts, and premium personal coaching.
              Choose the package that works best for your fitness program below.
            </p>

            <form onSubmit={handlePurchaseMembership} className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-xs font-semibold text-gray-500 mb-1">Select Tier</label>
                  <select
                    value={subType}
                    onChange={e => setSubType(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs bg-white focus:ring-1 focus:ring-blue-500"
                  >
                    <option value="Monthly">Monthly Membership Plan</option>
                    <option value="Annual">Annual Membership Plan</option>
                    <option value="Premium">Premium Access Plan</option>
                  </select>
                </div>
                <div>
                  <label className="block text-xs font-semibold text-gray-500 mb-1">Duration (Months)</label>
                  <select
                    value={subDuration}
                    onChange={e => setSubDuration(Number(e.target.value))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs bg-white focus:ring-1 focus:ring-blue-500"
                  >
                    <option value={1}>1 Month</option>
                    <option value={3}>3 Months</option>
                    <option value={6}>6 Months</option>
                    <option value={12}>12 Months</option>
                  </select>
                </div>
              </div>
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2.5 rounded-lg text-xs transition"
              >
                Activate Subscription (Complimentary / Free)
              </button>
            </form>

            {/* List of members - Admins / Receptionists only */}
            {canSeeAllMembers && (
              <div className="pt-4 border-t space-y-4">
                <h3 className="text-base font-bold text-gray-800 flex items-center">
                  <Users className="mr-2 text-blue-600" />
                  Active Registry (Internal Access)
                </h3>
                <div className="overflow-x-auto">
                  <table className="w-full text-left text-xs text-gray-500">
                    <thead className="bg-gray-50 text-gray-700 uppercase font-bold text-[10px]">
                      <tr>
                        <th className="p-3">Full Name</th>
                        <th className="p-3">Email</th>
                        <th className="p-3">Subscription</th>
                        <th className="p-3">Expires</th>
                        <th className="p-3">Status</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                      {allMembers.map(m => (
                        <tr key={m.id} className="hover:bg-gray-50">
                          <td className="p-3 font-bold text-gray-800">{m.fullName}</td>
                          <td className="p-3">{m.email}</td>
                          <td className="p-3 font-semibold text-blue-600">{m.membershipType}</td>
                          <td className="p-3">{new Date(m.membershipEndDate).toLocaleDateString()}</td>
                          <td className="p-3">
                            <span className={`px-2 py-0.5 rounded font-bold text-[10px] ${
                              m.isMembershipActive ? 'bg-emerald-100 text-emerald-800' : 'bg-red-100 text-red-800'
                            }`}>
                              {m.isMembershipActive ? 'Active' : 'Expired'}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};
