import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import type { Branch, Facility } from '../types/branch';
import { MapPin, Phone, Building2, Plus, Trash2, Dumbbell, ShieldAlert, CheckCircle2 } from 'lucide-react';

const API_URL = 'http://localhost:5264/api';

export const BranchView: React.FC = () => {
  const { token, user } = useAuth();
  const [branches, setBranches] = useState<Branch[]>([]);
  const [facilities, setFacilities] = useState<Record<string, Facility[]>>({});
  const [selectedBranch, setSelectedBranch] = useState<Branch | null>(null);

  // Form states - Branch
  const [newBranchName, setNewBranchName] = useState('');
  const [newBranchAddress, setNewBranchAddress] = useState('');
  const [newBranchPhone, setNewBranchPhone] = useState('');

  // Form states - Facility
  const [newFacName, setNewFacName] = useState('');
  const [newFacType, setNewFacType] = useState('Gym');
  const [newFacCapacity, setNewFacCapacity] = useState(10);
  const [newFacPrice, setNewFacPrice] = useState(0);

  // States
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // Permissions Check
  const canManageBranch = user && (user.role === 'Super Admin' || user.role === 'System Owner');
  const canManageFacility = user && (user.role === 'Super Admin' || user.role === 'System Owner' || user.role === 'Branch Manager');

  const fetchBranches = async () => {
    try {
      const res = await fetch(`${API_URL}/branch`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Failed to load branches.');
      const data = await res.json();
      setBranches(data);
      if (data.length > 0 && !selectedBranch) {
        setSelectedBranch(data[0]);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const fetchFacilities = async (branchId: string) => {
    try {
      const res = await fetch(`${API_URL}/branch/${branchId}/facilities`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Failed to load facilities.');
      const data = await res.json();
      setFacilities(prev => ({ ...prev, [branchId]: data }));
    } catch (err: any) {
      setError(err.message);
    }
  };

  useEffect(() => {
    if (token) {
      fetchBranches();
    }
  }, [token]);

  useEffect(() => {
    if (token && selectedBranch) {
      fetchFacilities(selectedBranch.id);
    }
  }, [token, selectedBranch]);

  const handleCreateBranch = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);
    try {
      const res = await fetch(`${API_URL}/branch`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          name: newBranchName,
          address: newBranchAddress,
          phoneNumber: newBranchPhone
        })
      });
      if (!res.ok) throw new Error('Could not create branch. Check permissions.');
      const created = await res.json();
      setBranches(prev => [...prev, created]);
      setSelectedBranch(created);
      setSuccess('Branch created successfully!');
      setNewBranchName('');
      setNewBranchAddress('');
      setNewBranchPhone('');
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteBranch = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this branch?')) return;
    setError(null);
    setSuccess(null);
    try {
      const res = await fetch(`${API_URL}/branch/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Could not delete branch.');
      setBranches(prev => prev.filter(b => b.id !== id));
      if (selectedBranch?.id === id) {
        setSelectedBranch(null);
      }
      setSuccess('Branch deleted successfully.');
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleCreateFacility = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedBranch) return;
    setLoading(true);
    setError(null);
    setSuccess(null);
    try {
      const res = await fetch(`${API_URL}/branch/facilities`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          branchId: selectedBranch.id,
          name: newFacName,
          type: newFacType,
          capacity: Number(newFacCapacity),
          pricePerHour: Number(newFacPrice)
        })
      });
      if (!res.ok) throw new Error('Could not create facility.');
      const created = await res.json();
      setFacilities(prev => ({
        ...prev,
        [selectedBranch.id]: [...(prev[selectedBranch.id] || []), created]
      }));
      setSuccess('Facility added successfully!');
      setNewFacName('');
      setNewFacCapacity(10);
      setNewFacPrice(0);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteFacility = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this facility?')) return;
    if (!selectedBranch) return;
    setError(null);
    setSuccess(null);
    try {
      const res = await fetch(`${API_URL}/branch/facilities/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Could not delete facility.');
      setFacilities(prev => ({
        ...prev,
        [selectedBranch.id]: (prev[selectedBranch.id] || []).filter(f => f.id !== id)
      }));
      setSuccess('Facility deleted successfully.');
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
      {/* Sidebar - Branches List & Create Form */}
      <div className="lg:col-span-1 space-y-6">
        <div className="bg-white p-5 rounded-xl border border-gray-100 shadow-sm">
          <h3 className="text-lg font-bold text-gray-800 mb-4 flex items-center">
            <Building2 className="mr-2 text-blue-600" />
            Branch Locations
          </h3>

          {branches.length === 0 ? (
            <div className="py-8 text-center text-gray-400 text-sm">
              No branch locations found.
            </div>
          ) : (
            <div className="space-y-2 max-h-96 overflow-y-auto">
              {branches.map(b => (
                <div
                  key={b.id}
                  onClick={() => setSelectedBranch(b)}
                  className={`p-3.5 rounded-lg cursor-pointer transition border flex justify-between items-center ${
                    selectedBranch?.id === b.id
                      ? 'bg-blue-50 border-blue-200 text-blue-900'
                      : 'bg-gray-50 hover:bg-gray-100 border-gray-100 text-gray-700'
                  }`}
                >
                  <div className="space-y-0.5">
                    <p className="font-bold text-sm">{b.name}</p>
                    <p className="text-xs text-gray-400 flex items-center">
                      <MapPin className="h-3 w-3 mr-1" />
                      {b.address}
                    </p>
                  </div>
                  {canManageBranch && (
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        handleDeleteBranch(b.id);
                      }}
                      className="text-gray-400 hover:text-red-500 p-1 rounded-md transition"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Create Branch Form */}
        {canManageBranch && (
          <div className="bg-white p-5 rounded-xl border border-gray-100 shadow-sm">
            <h4 className="text-sm font-bold text-gray-800 mb-3 flex items-center">
              <Plus className="mr-1.5 text-blue-600 h-4 w-4" />
              Add New Branch
            </h4>
            <form onSubmit={handleCreateBranch} className="space-y-3">
              <div>
                <label className="block text-xs font-semibold text-gray-500 mb-1">Branch Name</label>
                <input
                  type="text"
                  required
                  value={newBranchName}
                  onChange={e => setNewBranchName(e.target.value)}
                  className="w-full px-3 py-1.5 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                  placeholder="e.g. Downtown Arena"
                />
              </div>
              <div>
                <label className="block text-xs font-semibold text-gray-500 mb-1">Address</label>
                <input
                  type="text"
                  required
                  value={newBranchAddress}
                  onChange={e => setNewBranchAddress(e.target.value)}
                  className="w-full px-3 py-1.5 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                  placeholder="e.g. 101 Broadway Ave"
                />
              </div>
              <div>
                <label className="block text-xs font-semibold text-gray-500 mb-1">Phone Number</label>
                <input
                  type="text"
                  value={newBranchPhone}
                  onChange={e => setNewBranchPhone(e.target.value)}
                  className="w-full px-3 py-1.5 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                  placeholder="e.g. 555-0192"
                />
              </div>
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 rounded-lg text-xs transition"
              >
                Create Branch Location
              </button>
            </form>
          </div>
        )}
      </div>

      {/* Main Section - Facilities list & Add Form */}
      <div className="lg:col-span-2 space-y-6">
        {/* Alerts */}
        {success && (
          <div className="p-3 bg-emerald-50 border border-emerald-200 text-emerald-700 rounded-lg flex items-center text-xs">
            <CheckCircle2 className="mr-2 h-4 w-4 flex-shrink-0" />
            {success}
          </div>
        )}

        {error && (
          <div className="p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg flex items-center text-xs">
            <ShieldAlert className="mr-2 h-4 w-4 flex-shrink-0" />
            {error}
          </div>
        )}

        {selectedBranch ? (
          <div className="space-y-6">
            <div className="bg-white p-6 rounded-xl border border-gray-100 shadow-sm">
              <div className="flex justify-between items-start mb-6">
                <div>
                  <h2 className="text-2xl font-black text-gray-900">{selectedBranch.name}</h2>
                  <p className="text-sm text-gray-500 flex items-center mt-1">
                    <MapPin className="h-4 w-4 mr-1 text-gray-400" />
                    {selectedBranch.address}
                    {selectedBranch.phoneNumber && (
                      <span className="flex items-center ml-4">
                        <Phone className="h-4 w-4 mr-1 text-gray-400" />
                        {selectedBranch.phoneNumber}
                      </span>
                    )}
                  </p>
                </div>
              </div>

              <h3 className="text-base font-bold text-gray-800 mb-4 flex items-center border-b pb-2">
                <Dumbbell className="mr-2 h-5 w-5 text-blue-600" />
                Available Facilities (Courts, Turfs & Gym Areas)
              </h3>

              {!(facilities[selectedBranch.id]) || facilities[selectedBranch.id].length === 0 ? (
                <div className="py-12 text-center text-gray-400 text-sm">
                  No courts, turfs, or gym rooms registered at this branch yet.
                </div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {facilities[selectedBranch.id].map(f => (
                    <div key={f.id} className="p-4 border border-gray-100 rounded-xl bg-gray-50 flex justify-between items-center">
                      <div>
                        <p className="font-bold text-gray-800 text-sm">{f.name}</p>
                        <div className="flex space-x-2 mt-1">
                          <span className="text-[10px] font-bold px-2 py-0.5 rounded bg-blue-100 text-blue-800 uppercase">
                            {f.type}
                          </span>
                          <span className="text-[10px] font-bold px-2 py-0.5 rounded bg-gray-200 text-gray-700">
                            Cap: {f.capacity}
                          </span>
                        </div>
                      </div>
                      {canManageFacility && (
                        <button
                          onClick={() => handleDeleteFacility(f.id)}
                          className="text-gray-400 hover:text-red-500 p-1 rounded transition"
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* Create Facility Form */}
            {canManageFacility && (
              <div className="bg-white p-6 rounded-xl border border-gray-100 shadow-sm">
                <h3 className="text-base font-bold text-gray-800 mb-4 flex items-center">
                  <Plus className="mr-2 text-blue-600" />
                  Add Facility to {selectedBranch.name}
                </h3>
                <form onSubmit={handleCreateFacility} className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-xs font-semibold text-gray-500 mb-1">Facility Name</label>
                    <input
                      type="text"
                      required
                      value={newFacName}
                      onChange={e => setNewFacName(e.target.value)}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                      placeholder="e.g. Court A / AstroTurf 1"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-semibold text-gray-500 mb-1">Facility Type</label>
                    <select
                      value={newFacType}
                      onChange={e => setNewFacType(e.target.value)}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs bg-white focus:ring-1 focus:ring-blue-500"
                    >
                      <option value="Gym">Gym Area</option>
                      <option value="Turf">AstroTurf Pitch</option>
                      <option value="Court">Hard Court / Court</option>
                    </select>
                  </div>
                  <div>
                    <label className="block text-xs font-semibold text-gray-500 mb-1">Capacity (Max Persons)</label>
                    <input
                      type="number"
                      required
                      value={newFacCapacity}
                      onChange={e => setNewFacCapacity(Number(e.target.value))}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-semibold text-gray-500 mb-1">Price Per Hour (Metadata, USD)</label>
                    <input
                      type="number"
                      required
                      value={newFacPrice}
                      onChange={e => setNewFacPrice(Number(e.target.value))}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                    />
                  </div>
                  <div className="md:col-span-2 pt-2">
                    <button
                      type="submit"
                      disabled={loading}
                      className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2.5 rounded-lg text-xs transition"
                    >
                      Add Facility
                    </button>
                  </div>
                </form>
              </div>
            )}
          </div>
        ) : (
          <div className="p-12 text-center text-gray-400 border border-dashed rounded-xl">
            Select or create a branch location to manage its facilities.
          </div>
        )}
      </div>
    </div>
  );
};
