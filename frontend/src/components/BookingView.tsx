import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import type { Branch, Facility } from '../types/branch';
import type { Booking } from '../types/booking';
import { Calendar, Clock, XCircle, CheckCircle2, ShieldAlert } from 'lucide-react';

const API_URL = 'http://localhost:5264/api';

export const BookingView: React.FC = () => {
  const { token, user } = useAuth();

  // Reference lists
  const [branches, setBranches] = useState<Branch[]>([]);
  const [selectedBranch, setSelectedBranch] = useState<Branch | null>(null);
  const [facilities, setFacilities] = useState<Facility[]>([]);
  const [selectedFacility, setSelectedFacility] = useState<Facility | null>(null);

  // Core Booking state
  const [bookings, setBookings] = useState<Booking[]>([]);

  // Booking Form fields
  const [bookingDate, setBookingDate] = useState(new Date().toISOString().split('T')[0]);
  const [startTime, setStartTime] = useState('09:00');
  const [endTime, setEndTime] = useState('10:00');
  const [notes, setNotes] = useState('');

  // UI States
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // Load active branches
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

  // Load facilities for selected branch
  const fetchFacilities = async (branchId: string) => {
    try {
      const res = await fetch(`${API_URL}/branch/${branchId}/facilities`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Failed to load facilities.');
      const data = await res.json();
      setFacilities(data);
      if (data.length > 0) {
        setSelectedFacility(data[0]);
      } else {
        setSelectedFacility(null);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  // Load my or all bookings
  const fetchMyBookings = async () => {
    try {
      const isPrivileged = user && (user.role === 'Super Admin' || user.role === 'System Owner' || user.role === 'Branch Manager' || user.role === 'Receptionist');
      const endpoint = isPrivileged ? '/booking' : '/booking/my';
      const res = await fetch(`${API_URL}${endpoint}`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Failed to fetch bookings.');
      const data = await res.json();
      setBookings(data);
    } catch (err: any) {
      setError(err.message);
    }
  };

  useEffect(() => {
    if (token) {
      fetchBranches();
      fetchMyBookings();
    }
  }, [token]);

  useEffect(() => {
    if (token && selectedBranch) {
      fetchFacilities(selectedBranch.id);
    }
  }, [token, selectedBranch]);

  const handleCreateBooking = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFacility) {
      setError('Please select a facility/turf/court first.');
      return;
    }
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const res = await fetch(`${API_URL}/booking`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          facilityId: selectedFacility.id,
          bookingDate,
          startTime,
          endTime,
          notes
        })
      });

      if (!res.ok) {
        const errData = await res.json().catch(() => ({}));
        throw new Error(errData.error || 'Booking overlap or system error occurred.');
      }

      setSuccess('Booking reserved successfully!');
      setNotes('');
      fetchMyBookings();
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleCancelBooking = async (id: string) => {
    if (!window.confirm('Are you sure you want to cancel this booking?')) return;
    setError(null);
    setSuccess(null);
    try {
      const res = await fetch(`${API_URL}/booking/${id}/cancel`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Failed to cancel the reservation.');
      setSuccess('Booking has been cancelled.');
      fetchMyBookings();
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
      {/* Interactive Booking form */}
      <div className="lg:col-span-1 space-y-6">
        <div className="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-4">
          <h3 className="text-lg font-bold text-gray-800 flex items-center border-b pb-2">
            <Calendar className="mr-2 text-blue-600 h-5 w-5" />
            Book Court or Turf
          </h3>

          <form onSubmit={handleCreateBooking} className="space-y-4">
            <div>
              <label className="block text-xs font-semibold text-gray-500 mb-1">Select Branch</label>
              <select
                value={selectedBranch?.id || ''}
                onChange={e => {
                  const b = branches.find(x => x.id === e.target.value);
                  if (b) setSelectedBranch(b);
                }}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs bg-white focus:ring-1 focus:ring-blue-500"
              >
                {branches.map(b => (
                  <option key={b.id} value={b.id}>{b.name}</option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-xs font-semibold text-gray-500 mb-1">Select Pitch/Court</label>
              <select
                value={selectedFacility?.id || ''}
                onChange={e => {
                  const f = facilities.find(x => x.id === e.target.value);
                  if (f) setSelectedFacility(f);
                }}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs bg-white focus:ring-1 focus:ring-blue-500"
              >
                {facilities.length === 0 ? (
                  <option value="">No facilities available</option>
                ) : (
                  facilities.map(f => (
                    <option key={f.id} value={f.id}>{f.name} ({f.type})</option>
                  ))
                )}
              </select>
            </div>

            <div>
              <label className="block text-xs font-semibold text-gray-500 mb-1">Booking Date</label>
              <input
                type="date"
                required
                value={bookingDate}
                onChange={e => setBookingDate(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
              />
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-xs font-semibold text-gray-500 mb-1">Start Time</label>
                <select
                  value={startTime}
                  onChange={e => setStartTime(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs bg-white focus:ring-1 focus:ring-blue-500"
                >
                  {Array.from({ length: 15 }, (_, i) => {
                    const hour = 8 + i;
                    const format = `${hour.toString().padStart(2, '0')}:00`;
                    return <option key={format} value={format}>{format}</option>;
                  })}
                </select>
              </div>
              <div>
                <label className="block text-xs font-semibold text-gray-500 mb-1">End Time</label>
                <select
                  value={endTime}
                  onChange={e => setEndTime(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs bg-white focus:ring-1 focus:ring-blue-500"
                >
                  {Array.from({ length: 15 }, (_, i) => {
                    const hour = 9 + i;
                    const format = `${hour.toString().padStart(2, '0')}:00`;
                    return <option key={format} value={format}>{format}</option>;
                  })}
                </select>
              </div>
            </div>

            <div>
              <label className="block text-xs font-semibold text-gray-500 mb-1">Additional Notes</label>
              <input
                type="text"
                value={notes}
                onChange={e => setNotes(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs focus:ring-1 focus:ring-blue-500"
                placeholder="e.g. Training session or friendly match"
              />
            </div>

            <button
              type="submit"
              disabled={loading || facilities.length === 0}
              className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2.5 rounded-lg text-xs transition"
            >
              Confirm Booking (Complimentary)
            </button>
          </form>
        </div>
      </div>

      {/* Booking Calendar List & Overview */}
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

        {/* Existing reservations list */}
        <div className="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-4">
          <h3 className="text-base font-bold text-gray-800 flex items-center border-b pb-2">
            <Clock className="mr-2 text-blue-600 h-5 w-5" />
            Reservations List & Schedules
          </h3>

          {bookings.length === 0 ? (
            <div className="py-12 text-center text-gray-400 text-xs border border-dashed rounded-xl">
              No active reservations or bookings found.
            </div>
          ) : (
            <div className="space-y-3">
              {bookings.map(b => (
                <div key={b.id} className="p-4 border rounded-xl flex justify-between items-center bg-gray-50">
                  <div className="space-y-1">
                    <div className="flex items-center space-x-2">
                      <span className="font-bold text-gray-800 text-sm">{b.facilityName}</span>
                      <span className="text-[10px] font-bold px-2 py-0.5 rounded bg-blue-100 text-blue-800 uppercase">
                        {b.facilityType}
                      </span>
                      <span className={`text-[10px] font-bold px-2 py-0.5 rounded ${
                        b.status === 'Confirmed' ? 'bg-emerald-100 text-emerald-800' : 'bg-red-100 text-red-800'
                      }`}>
                        {b.status}
                      </span>
                    </div>
                    <p className="text-xs text-gray-500 font-medium">
                      Date: {new Date(b.bookingDate).toLocaleDateString()} | Hours: {b.startTime} - {b.endTime}
                    </p>
                    <p className="text-xs text-gray-400">Reserved for: <span className="font-semibold">{b.memberName}</span></p>
                    {b.notes && <p className="text-[11px] text-gray-400 italic">Notes: "{b.notes}"</p>}
                  </div>

                  {b.status === 'Confirmed' && (
                    <button
                      onClick={() => handleCancelBooking(b.id)}
                      className="text-red-500 hover:text-red-700 font-bold text-xs flex items-center space-x-1"
                    >
                      <XCircle className="h-4 w-4" />
                      <span>Cancel</span>
                    </button>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
