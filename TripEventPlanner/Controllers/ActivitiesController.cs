﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TripEventPlanner.Data;
using TripEventPlanner.Models;

namespace TripEventPlanner.Controllers {
    public class ActivitiesController : Controller {
        private readonly ItravelPlannerDBContext _context;

        public ActivitiesController( ItravelPlannerDBContext context ) {
            _context = context;
        }

        public async Task<IActionResult> Index( string searchString, string activityType, string location ) {

            ViewData["CurrentFilter"] = searchString;
            var activityTypeData = _context.ActivityTypes;
            ViewData["activityTypeFilter"] = activityTypeData;
            ViewData["activituType"] = activityType;
            var locationData = _context.Locations;
            ViewData["locationFilter"] = locationData;
            ViewData["location"] = location;

            var query = _context.Activities
                    .Where(s => s.Name.Contains(searchString) && s.ActivityType.Name.Contains(activityType))
                    .Include(a => a.Location).Where(s => location == s.Location.Name)
                    //.Where(n => n.Location.Name == location)
                    .Include(s => s.ActivityType);

            var activity = _context.Activities.Include(a => a.ActivityType).Include(a => a.Location).Include(s => s.ActivityType);
            var stringEmty = !String.IsNullOrEmpty(searchString);

            if( activityType == "Attraction" & stringEmty ) {
                activity = query;
            }
            if( activityType == "Event" & stringEmty ) {
                activity = query;
            }
            if( activityType == "Restaurant" & stringEmty ) {
                activity = query;
            }

            return View(await activity.AsNoTracking().ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details( short? id ) {
            if( id == null ) {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(a => a.ActivityType)
                .Include(a => a.Location)
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if( activity == null ) {
                return NotFound();
            }

            return View(activity);
        }

        // GET: Activities/Create
        public IActionResult Create() {
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "ActivityTypeId", "Name");
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name");
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( [Bind("ActivityId,Name,Description,Address,Price,ActivityTypeId,ImageUrl,StartDate,EndDate,LocationId")] Activity activity ) {
            if( ModelState.IsValid ) {
                _context.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "ActivityTypeId", "Name", activity.ActivityTypeId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name", activity.LocationId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit( short? id ) {
            if( id == null ) {
                return NotFound();
            }

            var activity = await _context.Activities.FindAsync(id);
            if( activity == null ) {
                return NotFound();
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "ActivityTypeId", "Name", activity.ActivityTypeId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name", activity.LocationId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( short id, [Bind("ActivityId,Name,Description,Address,Price,ActivityTypeId,ImageUrl,StartDate,EndDate,LocationId")] Activity activity ) {
            if( id != activity.ActivityId ) {
                return NotFound();
            }

            if( ModelState.IsValid ) {
                try {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch( DbUpdateConcurrencyException ) {
                    if( !ActivityExists(activity.ActivityId) ) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "ActivityTypeId", "Name", activity.ActivityTypeId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name", activity.LocationId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete( short? id ) {
            if( id == null ) {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(a => a.ActivityType)
                .Include(a => a.Location)
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if( activity == null ) {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed( short id ) {
            var activity = await _context.Activities.FindAsync(id);
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists( short id ) {
            return _context.Activities.Any(e => e.ActivityId == id);
        }
    }
}